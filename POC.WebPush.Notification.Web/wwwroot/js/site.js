// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

if (!('serviceWorker' in navigator)) {
    console.log("Service Worker isn't supported on this browser, disable or hide UI.");    
}

if (!('PushManager' in window)) {
    console.log("Push isn't supported on this browser, disable or hide UI.");    
}

window.addEventListener('load', () => { registerServiceWorker() });

function registerServiceWorker() {
    return navigator.serviceWorker
        .register('../service-worker.js')
        .then(function (registration) {
            console.log('Service worker successfully registered.');
            return registration;
        })
        .catch(function (err) {
            console.error('Unable to register service worker.', err);
        });
}

function askPermission() {
    return new Promise(function (resolve, reject) {
        const permissionResult = Notification.requestPermission(function (result) {
            resolve(result);
        });

        if (permissionResult) {
            permissionResult.then(resolve, reject);
        }
    }).then(function (permissionResult) {
        if (permissionResult !== 'granted') {
            throw new Error("We weren't granted permission.");
        }
    });
}

function subscribeUserToPush() {
    return navigator.serviceWorker
        .register('/service-worker.js')
        .then(function (registration) {
            const subscribeOptions = {
                userVisibleOnly: true,
                applicationServerKey: 'BDZqA7FqyfqdqQP0VNZ-wLXzGChpwPb0AFwv0t-nu3vTZpy-m5-z3ut-AmdrMrYO0TkITvzaw1W9Dt2sLBtO6qs'
            };

            return registration.pushManager.subscribe(subscribeOptions);
        })
        .then(function (pushSubscription) {
            console.log(
                'Received PushSubscription: ',
                JSON.stringify(pushSubscription),
            );
            return pushSubscription;
        });
}




