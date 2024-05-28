import Calendar from './calendar.js';

const calendarContainer = document.getElementById('calendar-container');
const id = calendarContainer.dataset.userId;
const token = calendarContainer.dataset.token;

if (userId !== null) {
    fetch(`https://trackingprojectwebappservice20240505190044.azurewebsites.net/api/${id}`, {
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
