var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors((options) => {
    options.AddPolicy("DevCors",(corsBuilder) => {
        corsBuilder.WithOrigins("http://localhost:4200","http://localhost:3000","http://localhost:4500")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else{
    app.UseHttpsRedirection();
}



// app.MapGet("/weatherforecast", () =>
// {
    
// })
// .WithName("GetWeatherForecast")
// .WithOpenApi();

app.MapControllers();

app.Run();


