// Auf die Uhr im HTML zugreifen 
//   und in einer Variablen speichern
const clockElem = document.getElementById('clock');
// Aktuelle Uhrzeit ermitteln => Funktion
function getTime() {
    let date = new Date();
    let time = date.toLocaleTimeString();
    // clockElem.innerText => Gilt für alle Formelemente
    // clockElem.innerHTML => Alles was kein Formelement ist
    clockElem.innerHTML = time;
}
// LOOP: Ermittel die Uhrzeit
//   und setzen den Textwert der Uhr auf die aktuelle Zeit
setInterval(getTime, 1000);
// setInterval(() => {
//     let date = new Date();
//     let time = date.toLocaleTimeString();
//     clockElem.innerHTML = time;
// }, 1000);

const jokeElem = document.getElementById('joke');
// C# => List<Person> persons = new List<Person>();
fetch('https://api.chucknorris.io/jokes/random?category=dev')
    .then(httpResponse => {
        return httpResponse.json();
    })
    .then(joke => {
        jokeElem.innerHTML = joke.value;
    });