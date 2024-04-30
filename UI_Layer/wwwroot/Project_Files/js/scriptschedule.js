document.addEventListener("DOMContentLoaded", function() {
    const days = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];
    const hours = ["9AM", "10AM", "11AM", "12PM", "1PM", "2PM", "3PM", "4PM", "5PM"];
  
    const scheduleBody = document.getElementById("scheduleBody");
  
    hours.forEach(hour => {
      const row = document.createElement("tr");
      const timeCell = document.createElement("td");
      timeCell.textContent = hour;
      row.appendChild(timeCell);
  
      days.forEach(day => {
        const cell = document.createElement("td");
        const textarea = document.createElement("textarea");
        textarea.setAttribute("id", `${day}-${hour}`);
        cell.appendChild(textarea);
        row.appendChild(cell);
      });
  
      scheduleBody.appendChild(row);
    });
  
    // Display current day and time
    function updateCurrentDay() {
      const currentDate = new Date();
      const options = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric', hour: 'numeric', minute: 'numeric', second: 'numeric' };
      const formattedDate = currentDate.toLocaleDateString('en-US', options);
      document.getElementById("currentDay").textContent = formattedDate;
    }
  
    updateCurrentDay();
    setInterval(updateCurrentDay, 1000); // Update every second
  });
  