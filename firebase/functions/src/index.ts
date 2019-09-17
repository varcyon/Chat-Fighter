import * as functions from 'firebase-functions'
import * as admin from 'firebase-admin'
admin.initializeApp()


exports.getTwitchStreamers = functions.https.onRequest(async (request, response) => {
    try {
        const snapshot = await admin.firestore().collection(`TwitchStreamers`).get()
        const results = Array()
        snapshot.forEach(docSnap => {
            const data = docSnap.data()
            results.push(data)
        })
        response.send(results)
    } catch (error) {
        //handle the error
        console.log(error)
        response.status(500).send(error)
    }
})





exports.doesChannelExist = functions.https.onCall(async (data) => {
    const db = admin.firestore()
    const channel = data.channel
    const platform = data.platform
    const channelData = JSON.parse(data.channelData)
    // console.log(channelData)
    try {
        const DBdata = await admin.firestore().collection(`${platform}Streamers`).doc(`${channel}`).get()
        if (!DBdata.exists) {
            await db.collection(`${platform}Streamers`).doc(`${channel}`).set(channelData)
            return {
                streamerExists: "noStreamer"
            }
        } else {
            console.log("doc exists")
            // await db.collection(`${platform}Streamers`).doc(`${channel}`).set(channelData)
            return {
                streamerExists: "streamer"
            }
        }
    } catch (error) {
        console.log(error)
    }
    return {
        fuctionran: "Does channel Exist has run"
    }
})

exports.UserJoinedQuery = functions.https.onCall(async (data) => {
    const db = admin.firestore()
    const channel = data.channel
    const platform = data.platform
    const UserName = data.user
    try {
        const playerDoc = await db.collection(`${platform}Streamers`).doc(`${channel}`).collection("Players").doc(`${UserName}`).get()
        if (playerDoc.exists) {
            //player exists
                const playerData = playerDoc.data()
                console.log(playerData)
                return{
                    results: "data",
                    playerData: playerData
                }
        } else if(!playerDoc.exists){
            const userDoc = await db.collection("Users").doc(`${UserName}`).get()
            if (userDoc.exists) {
                return {
                    results: "1"//"add Player and update users fighters"
                }
            } else {
                return {
                    results: "2"//"add User & Player"
                }
            }
        }
    } catch (error) {
        console.log(error)
    }
    return {
    }
})



exports.QueryChannelsDBPlayers = functions.https.onCall(async (data) => {
    const db = admin.firestore()
    const channel = data.channel
    const platform = data.platform
    const playersToUnity = new Array
    try {
        const players = await db.collection(`${platform}Streamers`).doc(`${channel}`).collection("Players").get()
        console.log(players)
        players.forEach(async player => {
            playersToUnity.push(player.data())
        })
    } catch (error) {
        console.log(error)
    }
    return {
        playersFromDB: playersToUnity
    }
})






exports.AddNewUsers = functions.https.onCall(async (data) => {
    const db = admin.firestore()
    const jsonObj = JSON.parse(data.dataFromUnity)
    const channel = data.channel
    try {
        jsonObj.forEach(async user  => {
            const userExist = await db.collection("Users").doc(`${user.UserName}`).get()
            if (!userExist.exists) {
                let setDoc = await db.collection("Users").doc(`${user.UserName}`).set(user)
                setDoc=setDoc
            } else {
                let updoc = await db.collection("Users").doc(`${user.UserName}`).update({
                    Fighters: admin.firestore.FieldValue.arrayUnion(`${channel}`)
                })
                updoc=updoc
            }
        })
    } catch (error) {
        console.log(error)
    }
    return {
        fuctionran: "Add new Users has run."
    }
})


exports.chatPlayersCoinUpdate = functions.https.onCall(async (data) => {
    const db = admin.firestore()
    const channel = data.channel
    const platform = data.platform
    const CPU = data.CPU
    const jsonObj = JSON.parse(data.CPData)
    try {
        jsonObj.forEach(async player => {
            let updateDoc =  await db.collection(`${platform}Streamers`).doc(`${channel}`).collection("Players").doc(`${player.UserName}`).update({
               Coin: admin.firestore.FieldValue.increment(CPU)
            })
            console.log(updateDoc)
        })
    } catch (error) {
        console.log(error)
    }
    return {
        fuctionran: CPU +" Coins have been given"
    }
})


exports.AddNewPlayers = functions.https.onCall(async (data) => {
    const db = admin.firestore()
    const channel = data.channel
    const platform = data.platform
    const jsonObj = JSON.parse(data.dataFromUnity)
    try {
        jsonObj.forEach(async player => {
            const userExist = await db.collection(`${platform}Streamers`).doc(`${channel}`).collection("Players").doc(`${player.UserName}`).get()
            if (!userExist.exists) {
                console.log("doesn't exist")
                let setDoc = await db.collection(`${platform}Streamers`).doc(`${channel}`).collection("Players").doc(`${player.UserName}`).set(player)
                setDoc = setDoc
            } else {
                console.log("does exists")
            }
        })
    } catch (error) {
        console.log(error)
    }
    return {
        fuctionran: "Add new players has run."
    }
})









// export const getUsersFighters =
//     functions.https.onCall(async (data) => {

//         try {


//         } catch (error) {
//             console.log(error)
//         }
//     })

//     export const updateCoin =
//     functions.https.onCall(async (data) => {

//         try {


//         } catch (error) {
//             console.log(error)
//         }
//     })
//     export const updateExp =
//     functions.https.onCall(async (data) => {

//         try {


//         } catch (error) {
//             console.log(error)
//         }
//     })

//     export const updateItems =
//     functions.https.onCall(async (data) => {

//         try {


//         } catch (error) {
//             console.log(error)
//         }
//     })