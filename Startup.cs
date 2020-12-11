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

			app.Use(async (context, next) =>
			{
				context.Response.Headers.Add("X-Application-Purpose", "Timestamp Microservice");
				await next.Invoke();
			});

			app.Run(async context =>
			{
				await context.Response.WriteAsync("You got an extra header!");
			});
		}
	}
}