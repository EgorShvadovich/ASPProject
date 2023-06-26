namespace ASPProject.Middleware
{
    public static class MarkerExtension
    {
        public static IApplicationBuilder UseMarker(this IApplicationBuilder app)
        {
            return app.UseMiddleware<Marker>();
        }
    }
}
