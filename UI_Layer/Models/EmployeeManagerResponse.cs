﻿namespace UI_Layer.Models
{
    public class EmployeeManagerResponse
    {
        public string Message {  get; set; }
        public bool IsSuccess { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}