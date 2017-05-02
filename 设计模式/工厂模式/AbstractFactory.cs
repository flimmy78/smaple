﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication13
{
    class FactoryMethod
    {
        static void Main1(string[] args)
        {
            IFactory f = new factory2();
            Department d = f.CreateDepartment();
            User s = f.CreateUser();
            d.showDepartment();
            s.showUser();
            Console.Read();
        }
    }

    public  interface IFactory
    {
        User CreateUser();
        Department CreateDepartment();
    }

    public class factory1 : IFactory
    {
        public User CreateUser()
        {
            return new User1();
        }

        public Department CreateDepartment()
        {
            return new Department1();
        }
    }

    public class factory2 : IFactory
    {

        public User CreateUser()
        {
            return new User2();
        }

        public Department CreateDepartment()
        {
            return new Department2();
        }
    }


    public abstract class User
    {
        public abstract void showUser();
    }

    public abstract class Department
    {
        public abstract void showDepartment();
    }

    public class User1 : User
    {
        public override void showUser()
        {
            Console.WriteLine("User1");
        }
    }

    public class User2 : User
    {
        public override void showUser()
        {
            Console.WriteLine("User2");
        }
    }

    public class Department1 : Department
    {
        public override void showDepartment()
        {
            Console.WriteLine("Department1");
        }
    }

    public class Department2 : Department
    {
        public override void showDepartment()
        {
            Console.WriteLine("Department2");
        }
    }
}
