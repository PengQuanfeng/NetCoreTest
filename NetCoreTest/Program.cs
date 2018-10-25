using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Threading;
using Newtonsoft.Json;
using System.Linq;

namespace NetCoreTest
{
    [Serializable]
    public enum Grend
    {
        Male = 0,
        Emale = 1
    }

    public struct S
    {
        public string name;
    }


    [CustomClass]
    [Serializable]
    //[Description("this is a")]
    public class A
    {
        [Description("A的ID")]
        public Guid Id;
        [Description("A的Name")]
        public String Name;
    }

    [Serializable]
    public class B<T>
    {
        [Description("B的Number")]
        public String Number;
        [Description("B的No")]
        public long No;
        [Description("B的list")]
        public List<T> list;
        [Description("B的array")]
        public T[] array;
        [Description("B的Enum")]
        public Grend grend;
        [Description("B的A")]
        public A a;
    }

    public class CustomClassAttribute : Attribute
    {

    }

    public class Log
    {
        public Guid Id;
        public Guid GroupId;
        public String AuditName;
        public DateTime CreateTime;
        public DateTime UpdateTime;
        public String ClassName;

        public List<LogEntry> list;
    }

    public class LogEntry
    {
        public Guid Id;
        //public Log log; //JSON 序列化不能自引用
        public String FieldName;
        public String FieldDescription;
        public object FieldValue;
    }

    class Program
    {

        public static void FunA(List<A> a)
        {
            Console.WriteLine(nameof(A));

            ThreadPool.QueueUserWorkItem(Write, a);
        }

        public static void FunS(S s)
        {
            Console.WriteLine(nameof(S));

            ThreadPool.QueueUserWorkItem(Write, s);
        }


        public delegate void WaitCallback(object state);


        public static void FunB(A a, B<A> b)
        {
            Console.WriteLine(nameof(B<string>));

            ThreadPool.QueueUserWorkItem(Write, b);

        }

        public static void Write<T>(T t)
        {
            var id = Guid.NewGuid();
            Write(id, t);
        }

        public static void Write<T>(Guid perId, T t)
        {
            var log = new Log();
            log.Id = Guid.NewGuid();
            log.GroupId = perId;
            log.ClassName = t.GetType().Name;
            log.AuditName = "peng.quanfeng";
            log.CreateTime = DateTime.Now;
            log.UpdateTime = DateTime.Now;
            log.list = new List<LogEntry>();

            //var members = FormatterServices.GetSerializableMembers(t.GetType());
            //var values = FormatterServices.GetObjectData(t, members);



            var fields = t.GetType().GetFields();
            foreach (var field in fields)
            {
                var value = field.GetValue(t);
                var length = field.FieldType.GetCustomAttributes(typeof(CustomClassAttribute), false).Length;

                if (length > 0)
                {
                    Write(perId, value);
                }
                else
                {
                    Console.WriteLine(field.Name + ":" + value);

                    var logEntry = new LogEntry();
                    logEntry.Id = Guid.NewGuid();
                    logEntry.FieldName = field.Name;
                    var description = field.GetCustomAttributes(false)[0] as DescriptionAttribute;
                    logEntry.FieldDescription = description.Description;
                    logEntry.FieldValue = value;

                    //logEntry.log = log;
                    log.list.Add(logEntry);
                }
            }

            Map.Add(log);
        }

        public static HashSet<Log> Map = new HashSet<Log>();

        static void Main(string[] args)
        {
            var a = new A();
            a.Id = Guid.NewGuid();
            a.Name = "This is A Name";

            var descriptions = a.GetType().GetCustomAttributesData();
            var test = descriptions.Where(d => d.AttributeType == typeof(DescriptionAttribute)).FirstOrDefault()?.ConstructorArguments.FirstOrDefault().Value;

            var attributes = a.GetType().GetCustomAttributes(typeof(DescriptionAttribute), false);
            var attribute = attributes.FirstOrDefault() as DescriptionAttribute;
            var test2= attribute?.Description;

            Console.WriteLine(test);
            Console.WriteLine(test2);

            var b = new B<A>();
            b.No = 10005;
            b.list = new List<A>();
            b.list.Add(a);
            b.list.Add(a);
            b.list.Add(a);

            b.array = new A[10];
            b.array[0] = a;
            b.array[1] = a;
            b.array[2] = a;
            b.array[3] = a;
            b.array[4] = a;
            b.array[5] = a;
            b.array[6] = a;

            b.grend = Grend.Emale;

            b.Number = "This is B Number";
            b.a = a;

            var A = new List<A>();
            A.Add(a);
            A.Add(a);

            //S s = new S();
            //s.name = "test s";
            //FunS(s);

            FunA(A);
            FunB(a, b);

            Thread.Sleep(2000);

            Console.WriteLine("Below is Hash Map:");
            foreach (var log in Map)
            {
                Console.WriteLine(JsonConvert.SerializeObject(log));
                continue;

                Console.WriteLine("This is Log:");
                Console.WriteLine(log.Id);
                Console.WriteLine("This Group Id:" + log.GroupId);
                Console.WriteLine(log.ClassName);
                Console.WriteLine(log.AuditName);
                Console.WriteLine(log.CreateTime);
                Console.WriteLine(log.UpdateTime);

                foreach (var item in log.list)
                {
                    Console.WriteLine("This is LogEntry:");
                    Console.WriteLine(item.Id);
                    Console.WriteLine(item.FieldName + ":" + item.FieldDescription + ":" + JsonConvert.SerializeObject(item));
                }
            }

            Console.ReadLine();
        }
    }
}
