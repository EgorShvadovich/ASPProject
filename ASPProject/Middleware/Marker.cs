using ASPProject.Data;

namespace ASPProject.Middleware
{
    public class Marker
    {
        private readonly RequestDelegate _next;
        private static int _cnt;
        private static int _getCount;
        private static int _postCount;


        public Marker(RequestDelegate next)
        {
            _next = next;
            _cnt = 0;
            _getCount = 0;
            _postCount = 0;

        }

        public async Task InvokeAsync(HttpContext context, DataContext dataContext)
        {
            if (context.Request.Method == "GET")
            {
                _getCount++;
            }
            else if (context.Request.Method == "POST")
            {
                _postCount++;
            }

            context.Items.Add("marker", $"GET requests: {_getCount}, POST requests: {_postCount}");

            //context.Items.Add("marker", $"TheMarker,{dataContext.Users.Count()} users, {_cnt++} requests");
            await _next(context);
        }
    }
}
