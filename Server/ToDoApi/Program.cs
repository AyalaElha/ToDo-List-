using Microsoft.EntityFrameworkCore;
using ToDoApi;

var builder = WebApplication.CreateBuilder(args);
//var url;

// הוספת תמיכה ב-CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()  // אפשר לכל מקור
               .AllowAnyMethod() // אפשר כל שיטה (GET, POST, וכו')
               .AllowAnyHeader(); // אפשר כל כותרת
    });
});

// הגדרת החיבור למסד נתונים MySQL
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("tododb"),  // שימוש ב-connectionString מתוך הקובץ
        ServerVersion.Parse("8.0.32-mysql"))  // גרסת MySQL בה אתה משתמש
    );

var app = builder.Build();

// מפת דרכים
app.MapGet("/", () => "ToDo API");

// שליפת כל המשימות
app.MapGet("/tasks", async (ToDoDbContext db) =>
{
    try
    {
        return Results.Ok(await db.Items.ToListAsync());
    }
    catch (Exception ex)
    {
        // שגיאה בנתונים
        return Results.Problem("There was an error retrieving tasks. Please try again later.", statusCode: 500);
    }
});

// שליפת משימה לפי ID
app.MapGet("/tasks/{id:int}", async (int id, ToDoDbContext db) =>
{
    var task = await db.Items.FindAsync(id);
    return task is not null ? Results.Ok(task) : Results.NotFound();
});

// הוספת משימה חדשה
app.MapPost("/tasks", async (Item newTask, ToDoDbContext db) =>
{
    db.Items.Add(newTask);
    await db.SaveChangesAsync();
    return Results.Created($"/tasks/{newTask.Id}", newTask);
});

// עדכון משימה
app.MapPut("/tasks/{id:int}", async (int id, Item updatedTask, ToDoDbContext db) =>
{
    var task = await db.Items.FindAsync(id);
    if (task is null) return Results.NotFound();

    task.Name = updatedTask.Name;
    task.IsComplete = updatedTask.IsComplete;
    await db.SaveChangesAsync();
    return Results.Ok(task);
});

// מחיקת משימה
app.MapDelete("/tasks/{id:int}", async (int id, ToDoDbContext db) =>
{
    var task = await db.Items.FindAsync(id);
    if (task is null) return Results.NotFound();

    db.Items.Remove(task);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.UseRouting();
app.UseCors();
// app.UseEndpoints(endpoints =>
// {
//     endpoints.MapControllers();
// });

// הפעלת האפליקציה
app.Run();
