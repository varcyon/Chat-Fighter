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
    console.log(channelData)
    try {
        const DBdata = await admin.firestore().collection(`${platform}Streamers`).doc(`${channel}`).get()
        if (!DBdata.exists) {
            const setDoc = db.collection(`${platform}Streamers`).doc(`${channel}`).set(channelData)
            console.log("doc doesn't exists")
            console.log(setDoc)
            return {
                streamerExists: "noStreamer"
            }
        } else {
            console.log("doc exists")
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





exports.QueryChannelsCurrentPlayers = functions.https.onCall(async (data) => {
    const db = admin.firestore()
    const channel = data.channel
    const platform = data.platform
    let playersToUnity = new Array
    try {
        const players = await db.collection(`${platform}Streamers`).doc(`${channel}`).collection("Players").get()
        console.log(players)
        players.forEach(async player => {
            //    console.log(player.id, '=>', player.data())
            playersToUnity.push(player.data())

        })
        console.log(playersToUnity)
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
    console.log(channel)
    try {
        jsonObj.forEach(async user => {
            const userExist = await db.collection("Users").doc(`${user.UserName}`).get()
            if (!userExist.exists) {
                console.log("doesn't exist")
                const setDoc = await db.collection("Users").doc(`${user.UserName}`).set(user)
                console.log(setDoc)
            } else {
                console.log("does exists")
                console.log(channel)
                await db.collection("Users").doc(`${user.UserName}`).update({
                    Fighters: admin.firestore.FieldValue.arrayUnion(`${channel}`)
                    
                })
            }
        })
    } catch (error) {
        console.log(error)
    }
    return {
        fuctionran: "Add new Users has run."
    }
})





exports.AddNewPlayers = functions.https.onCall(async (data) => {
    const db = admin.firestore()
    const channel = data.channel
    const platform = data.platform
    const jsonObj = JSON.parse(data.dataFromUnity)
    try {
        jsonObj.forEach(async player => {
            const userExist = await db.collection(`${platform}Streamers`).doc(`${channel}`).collection("Players").doc(`${player.UserName}`).get();
            if (!userExist.exists) {
                console.log("doesn't exist")
                const setDoc = await db.collection(`${platform}Streamers`).doc(`${channel}`).collection("Players").doc(`${player.UserName}`).set(player)
                console.log(setDoc)
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