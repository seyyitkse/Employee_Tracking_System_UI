/// <reference path="calendar.js" />

import Calendar from './calendar.js';

export default class CustomCalendar extends Calendar {
    constructor(events) {
        super(events);
        this._addClickListener();
    }

    _addClickListener() {
        const calendarBody = document.getElementById('calendar-body');
        calendarBody.addEventListener('click', (event) => {
            const target = event.target;
            if (target.classList.contains('calendar-body-column') && !target.classList.contains('calendar-timeline-column')) {
                const day = target.dataset.day;
                const hour = target.parentElement.dataset.hour;
                const dateFrom = new Date(this.currentDate);
                dateFrom.setDate(dateFrom.getDate() + parseInt(day) - 1);
                dateFrom.setHours(parseInt(hour), 0, 0, 0);
                const dateTo = new Date(dateFrom);
                dateTo.setHours(dateTo.getHours() + 1);

                // Burada yeni bir olay oluşturabilir veya mevcut bir olayı güncelleyebilirsiniz.
                // Örnek olarak bir iletişim kutusu gösterebilir ve kullanıcının olay adını girmesini isteyebilirsiniz.
                const eventName = prompt('Etkinlik adını girin:');
                if (eventName) {
                    const newEvent = {
                        id: this._getRandomId(),
                        eventName: eventName,
                        dateFrom: dateFrom,
                        dateTo: dateTo,
                        bgColor: this._getRandomBgColorAndTextColor().bgColor,
                        textColor: this._getRandomBgColorAndTextColor().textColor
                    };
                    this.events.push(newEvent);
                    this.render();
                }
            }
        });
    }
}