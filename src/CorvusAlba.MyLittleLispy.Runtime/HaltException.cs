﻿using System;

namespace CorvusAlba.MyLittleLispy.Runtime
{
    public class HaltException : Exception
    {
        public int Code { get; set; }

        public HaltException(int code)
        {
            Code = code;
        }
    }
}