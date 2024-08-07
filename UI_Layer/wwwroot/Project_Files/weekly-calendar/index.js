import Calendar from './calendar.js';

const url = new URL(window.location.href);
const id = url.searchParams.get('id');

if (id !== null) {
    fetch(`https://trackingprojectwebappservice20240505190044.azurewebsites.net/api/ScheduleUser/${id}`)
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
    console.error('No id parameter in the URL');
}