self.addEventListener('fetch', function (event) { });

self.addEventListener('push', function (event) {
    const title = 'Push Notification';
    const options = {
        body: event.data.text()
    }        
    event.waitUntil(
        self.registration.showNotification(title, options)
    )
});