﻿using ASPProject.Data;

namespace ASPProject.Middleware
{
    public class Marker
    {
        private readonly RequestDelegate _next;
        private static int _cnt;

        public Marker(RequestDelegate next)
        {
            _next = next;
            _cnt = 0;
        }

        public async Task InvokeAsync(HttpContext context, DataContext dataContext)
        {
            context.Items.Add("marker", $"TheMarker,{dataContext.Users.Count()} users, {_cnt++} requests");
            await _next(context);
        }
    }
}