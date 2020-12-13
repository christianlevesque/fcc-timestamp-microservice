using System.Linq;
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
		}
	}
}