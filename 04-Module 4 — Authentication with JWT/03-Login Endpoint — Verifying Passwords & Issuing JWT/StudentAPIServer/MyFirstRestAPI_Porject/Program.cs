var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Adding CORS Before Build
builder.Services.AddCors(options =>
{
    options.AddPolicy("StudentApiPolicy", policy =>
    {
        policy.WithOrigins(
            "https://localhost:7217",
            "https://localhost:5215"
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//HTTPS Redirection Before Controller Mapping
app.UseHttpsRedirection();

app.UseCors("StudentApiPolicy");

app.UseAuthorization();




//--------------------//
//Controller Mapping
app.MapControllers();

app.Run();
