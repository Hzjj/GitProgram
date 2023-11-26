using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebService;
using WebService.Util;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public abstract class BaseAttribute : Attribute
    {
        public virtual string error { get; set; }
        public abstract bool Validate(object value);
    }

    /// <summary>
    /// 非空效验
    /// </summary>
    public class RequiredAttribute : BaseAttribute
    {
        public override string error
        {
            get
            {
                if (base.error != null)
                {
                    return base.error + "不能为空";
                }
                return "属性不能为空";

            }
            set => base.error = value;
        }
        public override bool Validate(object value)
        {
            return !(value == null);
        }
    }



    /// <summary>
    /// 字符串非空效验
    /// </summary>
    public class StringRequiredAttribute : BaseAttribute
    {
        public override string error
        {
            get
            {
                if (base.error != null)
                {
                    return base.error+ "不能为空";
                }
                return "属性不能为空";
            }
            set => base.error = value;
        }
        public override bool Validate(object value)
        {
            return !(value == null) && value.ToStr().Length > 0;
        }
    }

    /// <summary>
    /// 身份证号效验
    /// </summary>
    public class SFZHRequiredAttribute : BaseAttribute
    {
        public override string error
        {
            get
            {
                if (base.error != null)
                {
                    return base.error;
                }
                return "身份证号码格式非法,请检查身份证号码！。";
            }
            set => base.error = value;
        }
        public override bool Validate(object value)
        {
            return Tool.CheckIDCard(value.ToStr());
        }
    }

    /// <summary>
    /// 约束字符串的长度范围
    /// </summary>
    public class StringRangeAttribute : BaseAttribute
    {
        public int min { get; set; } = 0;
        public int max { get; set; } = 0;
        public bool isnull { get; set; } = false;
        public override string error
        {
            get             
            {
                
                return base.error+$"字符串长度范围{this.min}-{this.max}";
            }
            set => base.error = value;
        }
        public override bool Validate(object value)
        {
            if (isnull == true) 
            {
                if (value.ToStr().Length <= 0) 
                {
                    return false;
                }
            }
            int count = value.ToStr().Length;
            return max != 0 ? count <= this.max : count>=this.min;         
        }
    }

