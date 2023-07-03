// Create the Server-Template
var builder = WebApplication.CreateBuilder(args);
// Configure the Server

// Build the Server
var app = builder.Build();

// Route-Mapping
app.MapGet("/", () => "Hello World!");
// Beispiel für eine Route mit Query-Parametern
app.MapGet("/search", (string query) => $"Suchergebnis = '{query}'");
app.MapGet("/customer", () => {
    if(true) {
        return "CustomerList";
    }
    return "No Customers found";
});
// Beispiel für eine Route als Variable
app.MapGet("/customer/{id}", (int id) => $"Customer {id}");

// Start the Server
app.Run();
