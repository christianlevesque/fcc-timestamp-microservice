using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;

namespace timestamp
{
	public class Startup
	{
		private readonly IDictionary<string, string> _error = new Dictionary<string, string>
		{
			{"error", "Invalid Date"}
		};

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddCors(options =>
			{
				options.AddPolicy("AllowFcc", policy =>
				{
					policy.WithOrigins("https://www.freecodecamp.org");
					policy.WithMethods("GET");
					policy.WithHeaders(HeaderNames.Accept, HeaderNames.AcceptEncoding, HeaderNames.AcceptLanguage, HeaderNames.Connection, HeaderNames.Host, HeaderNames.Origin, HeaderNames.Referer, HeaderNames.UserAgent);
				});
			});
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseCors("AllowFcc");

			app.Use((context, next) =>
			{
				// Nginx is great...
				// but I don't want 2 location blocks just
				// to handle /api/timestamp and /api/timestamp/date
				// So if Nginx did the thing, strip the double slash
				if (context.Request.Path.ToString()
						   .StartsWith("//"))
				{
					context.Request.Path = context.Request.Path.ToString()
												  .Substring(1);
				}
				
				// Our app always returns JSON
				context.Response.ContentType = "application/json";
				return next.Invoke();
			});

			app.Use((context, next) =>
			{
				// If there is not a supplied date
				// move on to app.Run
				if (context.Request.Path == "/")
					return next.Invoke();

				// context.Request.Path is a PathString, not a string
				// so call ToString() on it 
				var date = context.Request.Path.ToString()
								  .Substring(1);
				var timestamp = GetTimestamp(date);
				
				// GetTimestamp returns null if the date couldn't be parsed
				if (timestamp == null)
					return context.Response.WriteAsync(JsonSerializer.Serialize(_error));

				return context.Response.WriteAsync(JsonSerializer.Serialize(timestamp));
			});

			app.Run(context =>  context.Response.WriteAsync(JsonSerializer.Serialize(new Timestamp())));
		}

		private Timestamp GetTimestamp(string dateString)
		{
			// If date can be parsed as long
			// it's a UNIX timestamp
			if (long.TryParse(dateString, out var unixDate))
			{
				var epoch = DateTime.UnixEpoch;
				var date1 = epoch.AddMilliseconds(unixDate)
								.ToUniversalTime();
				return new Timestamp(date1);
			}
			
			// If a date can be parsed as a string
			// it's a UTC date string
			if (DateTime.TryParse(dateString, out var date2))
				return new Timestamp(date2);

			// If a date can be parsed as a string
			// after replacing URL-encoded spaces with hyphens
			// it's a JavaScript-compatible non-UTC date string
			dateString = dateString.Replace("%20", "-");
			if (DateTime.TryParse(dateString, out var date3))
				return new Timestamp(date3);

			// It wasn't a valid date
			return null;
		}
	}
}