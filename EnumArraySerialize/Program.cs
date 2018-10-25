using System;
using System.Collections.Generic;

namespace EnumArraySerialize
{
    class Program
    {
        /// <summary>
        /// 老师标签
        /// </summary>
        [Flags]
        public enum TeacherTag
        {
            Special = 1,
            Expert = 2,
            Ordinary = 4,
            Primary = 8
        }

        public class Teacher
        {
            public Guid Id;
            public string Name;
            public TeacherTag TeacherTag;
        }

        static void Main(string[] args)
        {
            var teacherTag = TeacherTag.Special | TeacherTag.Expert | TeacherTag.Primary;

            var teacherTagInt = (int)(teacherTag);
            TeacherTag teacherTagEnum = (TeacherTag)teacherTagInt;

            var teacherTag1 = TeacherTag.Special;

            Teacher teacher = new Teacher()
            {
                Id = Guid.NewGuid(),
                Name = "测试枚举",
                TeacherTag = teacherTag
            };

            if (teacher.TeacherTag.HasFlag(TeacherTag.Special))
            {
                Console.WriteLine("This Teacher is Special.");
            }

            Console.WriteLine(teacherTag1);
            Console.WriteLine(teacherTag);

            Console.WriteLine(teacher.ToString());

            Console.ReadLine();
            Console.WriteLine("Hello World!");
        }
    }
}
