using System;

namespace FaceIDApp.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string EmployeeCode { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime HireDate { get; set; }
        public bool IsActive { get; set; }
        public bool HasFaceRegistered { get; set; }
        public byte[] FaceData { get; set; }
        public string ImagePath { get; set; }
    }
}
