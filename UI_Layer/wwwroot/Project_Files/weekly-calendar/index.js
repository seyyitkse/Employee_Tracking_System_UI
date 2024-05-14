import Calendar from './calendar.js';

fetch('http://localhost:27312/api/ScheduleUser')
    .then(response => response.json())
    .then(events => {
        const eventArray = events.map(event => {
            return {
                id: event.scheduleID,
                eventName: event.description,
                dateFrom: new Date(event.starttime).getTime(),
                dateTo: new Date(event.endtime).getTime()
            };
        });

        new Calendar(eventArray);
    })
    .catch(error => {
        console.error('Error fetching events:', error);
    });