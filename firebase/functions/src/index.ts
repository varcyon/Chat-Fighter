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
exports.doesStreamerExist = functions.https.onRequest(async (request, response) => {
    const channel = request.query.channel
    const platform = request.query.platform
    try {
        const data = await admin.firestore().collection(`${platform}Streamers`).doc(`${channel}`).get()
        if (!data.exists) {
            response.send("0")
        } else {
            response.send("1")
        }
    } catch (error) {
        console.log(error)
        response.status(500).send(error)
    }
})

exports.AddNewPlayersToStreamer = functions.https.onRequest((request, response) => {
    const dict = request.query.data
    console.log(dict)
})

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
            fighters.forEach(fighter => {
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
