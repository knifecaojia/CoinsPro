using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace Common
{
    public static class EnumExt
    {

        /// <summary>
        /// 取得枚举标注System.ComponentModel.DescriptionAttribute的描述信息 
        /// </summary>
        /// <param name="enumType">Type of the enum.</param>
        /// <param name="itemValue">The item value.</param>
        /// <returns></returns>
        public static string Description(this object @enum)
        {
            //EnumItem[] set = EnumItemSet.AsItemSet(@enum.GetType().UnderlyingSystemType).ToArray();

            EnumItem objItem =
                EnumItemSet.AsItemSet(@enum.GetType().UnderlyingSystemType)
                .Where(x => x.Name == @enum.ToString())
                .FirstOrDefault();

            if (objItem == null)
                return string.Empty;

            return objItem.Text;
        }
        /// <summary>
        /// 取得枚举标注System.ComponentModel.CategoryAttribute的描述信息 
        /// </summary>
        /// <param name="enumType">Type of the enum.</param>
        /// <param name="itemValue">The item value.</param>
        /// <returns></returns>
        public static string Category(this object @enum)
        {
            //EnumItem[] set = EnumItemSet.AsItemSet(@enum.GetType().UnderlyingSystemType).ToArray();

            EnumItem objItem =
                EnumItemSet.AsItemSet(@enum.GetType().UnderlyingSystemType)
                .Where(x => x.Name == @enum.ToString())
                .FirstOrDefault();

            if (objItem == null)
                return string.Empty;

            return objItem.Category;
        }

    }
    /// <summary>
    /// 枚举项
    /// </summary>
    public class EnumItem
    {
        private string _Value = string.Empty;
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        private string _Name = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private string _Text = string.Empty;
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get { return _Text; }
            set { _Text = value; }
        }
        private string _Category = string.Empty;
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Category
        {
            get { return _Category; }
            set { _Category = value; }
        }
    }

    public class EnumItemSet : IEnumerable<EnumItem>
    {
        private static readonly string ERROR_ARG = "类型必须是枚举";

        /// <summary>
        /// 将枚举转换为IEnumerable &lt;EnumItem &gt;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enum"></param>
        /// <returns></returns>
        public static EnumItemSet AsItemSet<T>(T @enum)
        {
            return new EnumItemSet(typeof(T));
        }
        /// <summary>
        ///  将枚举转换为IEnumerable &lt;EnumItem &gt;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static EnumItemSet AsItemSet<T>()
        {
            return new EnumItemSet(typeof(T));
        }
        /// <summary>
        /// 将枚举转换为IEnumerable &lt;EnumItem &gt;
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static EnumItemSet AsItemSet(Type type)
        {
            //todo 重构
            if (type == null) throw new ArgumentException("type");
            if (!type.IsEnum) throw new InvalidOperationException(ERROR_ARG);
            return new EnumItemSet(type);
        }

        private Type m_enumType = null;

        private EnumItemSet(Type type)
        {
            m_enumType = type;
        }

        /// <summary>
        /// 取得枚举器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<EnumItem> GetEnumerator()
        {
            Type typeDescription = typeof(DescriptionAttribute);
            Type typeCategory = typeof(CategoryAttribute);
            System.Reflection.FieldInfo[] fields = m_enumType.GetFields();
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsEnum == true)
                {
                    string description = "";
                    string category = "";
                    System.Enum enumValue = (System.Enum)m_enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null);
                    object[] arr = field.GetCustomAttributes(typeDescription, true);
                    if (arr.Length > 0)
                    {
                        DescriptionAttribute da = (DescriptionAttribute)arr[0];
                        description = da.Description;
                    }
                    else
                    {
                        description = field.Name;
                    }

                    arr = field.GetCustomAttributes(typeCategory, true);
                    if (arr.Length > 0)
                    {
                        CategoryAttribute ca = (CategoryAttribute)arr[0];
                        category = ca.Category;
                    }
                    else
                    {
                        category = field.Name;
                    }

                    yield return new EnumItem() { Name = enumValue.ToString(), Value = enumValue.ToString("d"), Text = description, Category = category };
                }
            }
        }

        /// <summary>
        /// 取得枚举器
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}
