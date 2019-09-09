import * as functions from 'firebase-functions'
import * as admin from 'firebase-admin'
admin.initializeApp()

// Create and Deploy Your First Cloud Functions
// https://firebase.google.com/docs/functions/write-firebase-functions

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
    let playersToUnity= new Array
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
    //check if user already exists
    //////if does exist, check if they exist on streamers player list
    ////////if they do.. do nothing
    //////if they do not exist on streamers player list just add to the Fighters array
    //
    //if not,add user to Users
    try {
        jsonObj.forEach(async user => {
            const userExist = await db.collection("Users").doc(`${user.UserName}`).get()
            if (!userExist.exists) {
                console.log("doesn't exist")
                const setDoc = await db.collection("Users").doc(`${user.UserName}`).set(user)
                console.log(setDoc)
            } else {
                console.log("does exists")
            }
            // console.log(user.Id)
            // console.log(user.DisplayName)
            // console.log(user.UserName)
            // console.log(user.ProfileUrl)
            // console.log(user.Fighters)d

        }
        )
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
    //check if user already exists
    //////if does exist, check if they exist on streamers player list
    ////////if they do.. do nothing
    //////if they do not exist on streamers player list just add to the Fighters array
    //
    //if not,add user to Users
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
            // console.log(user.Id)
            // console.log(user.DisplayName)
            // console.log(user.UserName)
            // console.log(user.ProfileUrl)
            // console.log(user.Fighters)d

        }
        )
    } catch (error) {
        console.log(error)
    }
    return {
        fuctionran: "Add new players has run."
    }
})

// Add a new document in collection "cities" with ID 'LA'
// let setDoc = db.collection('cities').doc('LA').set(data);




exports.getItems = functions.https.onRequest(async (request, response) => {
    const item = request.query.item
    try {
        const snapshot = await admin.firestore().doc(`Items/${item}`).get()
        const data = snapshot.data()
        response.send(data)
    } catch (error) {
        console.log(error)
        response.status(500).send(error)
    }
})


export const getUsersFighters =
    functions.https.onRequest(async (request, response) => {
        const userName = request.query.userName
        try {
            const fightersSnapshot = await admin.firestore().doc(`Users/${userName}`).get()
            const fighters = fightersSnapshot.data()!.fighters
            const promises = Array()
            fighters.forEach((fighter: any) => {
                const f = admin.firestore().doc(`TwitchStreamers/${fighter}/Fighters/${userName}`).get()
                promises.push(f)
            })
            const fightersDocs = await Promise.all(promises)
            const results = Array()
            fightersDocs.forEach(fighter => {
                const data = fighter.data()
                //data.userName = fighter.userName
                results.push(data)
            })
            response.send(results)
        } catch (error) {
            console.log(error)
            response.status(500).send(error)
        }
    })

    // exports.addNumbers = functions.https.onCall((data) => {
    //     // Numbers passed from the client.
    //     const firstNumber = data.firstNumber;
    //     const secondNumber = data.secondNumber;

    //     // Checking that attributes are present and are numbers.
    //     if (!Number.isFinite(firstNumber) || !Number.isFinite(secondNumber)) {
    //       // Throwing an HttpsError so that the client gets the error details.
    //       throw new functions.https.HttpsError('invalid-argument', 'The function ' +
    //           'must be called with two arguments "firstNumber" and "secondNumber" ' +
    //           'which must both be numbers.');
    //     }

    //     // returning result.
    //     return {
    //       firstNumber: firstNumber,
    //       secondNumber: secondNumber,
    //       operator: '+',
    //       operationResult: firstNumber + secondNumber,
    //     };
    //   });