namespace ASPProject.Middleware
{
    public static class MarkerExtension
    {
        public static IApplicationBuilder UseMarker(this IApplicationBuilder app)
        {
            return app.UseMiddleware<Marker>();
        }
        public static IApplicationBuilder UseMarker2(this IApplicationBuilder app)
        {
            return app.UseMiddleware<Marker2>();
        }
    }
}
