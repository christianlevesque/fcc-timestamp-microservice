using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace timestamp
{
	public class Startup
	{
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.Map("/trigger-map", builder =>
			{
				builder.Use(async (context, next) =>
				{
					context.Response.Headers.Add("X-Proof-Of-Middleware", "I was executed!");
					await next.Invoke();
				});
			});

			app.MapWhen(context => context.Request.Method == "POST", builder =>
			{
				builder.Use(async (context, next) =>
				{
					if (!context.Request.Headers.ContainsKey("X-Application-Purpose"))
						context.Response.Headers.Add("X-Application-Purpose", "Timestamp Microservice");

					await next.Invoke();
				});
			});

			app.Run(async context =>
			{
				await context.Response.WriteAsync("You got an extra header!");
			});
		}
	}
}