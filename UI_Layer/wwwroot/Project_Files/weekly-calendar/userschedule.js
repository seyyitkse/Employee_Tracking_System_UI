import Calendar from './calendar.js';

const calendarContainer = document.getElementById('calendar-container');
const userId = calendarContainer.dataset.userId; // Correctly get the userId from dataset
const token = calendarContainer.dataset.token;

console.log('Retrieved userId:', userId); // Log the userId for debugging
console.log('Retrieved token:', token); // Log the token for debugging

if (userId) { // Check if userId is not null or undefined
    fetch(`https://trackingprojectwebappservice20240505190044.azurewebsites.net/api/ScheduleUser/${userId}`, { // Update API request URL
        headers: {
            'Authorization': `Bearer ${token}`
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(schedule => {
            let eventArray = [];
            if (schedule.length > 0) {
                eventArray = schedule.map(event => {
                    return {
                        id: event.scheduleID,
                        eventName: event.description,
                        dateFrom: new Date(event.starttime).getTime(),
                        dateTo: new Date(event.endtime).getTime()
                    };
                });
            }

            new Calendar(eventArray);
        })
        .catch(error => {
            console.error('Error fetching events:', error);
        });
} else {
    console.error('No userId parameter in the URL');
}
