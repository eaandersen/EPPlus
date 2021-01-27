/*************************************************************************************************
  Required Notice: Copyright (C) EPPlus Software AB. 
  This software is licensed under PolyForm Noncommercial License 1.0.0 
  and may only be used for noncommercial purposes 
  https://polyformproject.org/licenses/noncommercial/1.0.0/

  A commercial license to use this software can be purchased at https://epplussoftware.com
 *************************************************************************************************
  Date               Author                       Change
 *************************************************************************************************
  01/27/2020         EPPlus Software AB       Initial release EPPlus 5
 *************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml;
using System.Globalization;

namespace OfficeOpenXml.Style.Dxf
{
    /// <summary>
    /// A fill in a differential formatting record
    /// </summary>
    public class ExcelDxfFill : DxfStyleBase
    {
        internal ExcelDxfFill(ExcelStyles styles)
            : base(styles)
        {
            PatternColor = new ExcelDxfColor(styles);
            BackgroundColor = new ExcelDxfColor(styles);
            Gradient = null;
        }
        ExcelFillStyle? _patternType;
        /// <summary>
        /// The pattern tyle
        /// </summary>
        public ExcelFillStyle? PatternType 
        { 
            get
            {
                return _patternType;
            }
            set
            {
                if(Style==eDxfFillStyle.GradientFill)
                {
                    throw new InvalidOperationException("Cant set Pattern Type when Style is set to GradientFill");
                }
                _patternType = value;
            }
        }
        /// <summary>
        /// The color of the pattern
        /// </summary>
        public ExcelDxfColor PatternColor { get; internal set; }
        /// <summary>
        /// The background color
        /// </summary>
        public ExcelDxfColor BackgroundColor { get; internal set; }
        /// <summary>
        /// The Id
        /// </summary>
        protected internal override string Id
        {
            get
            {
                return GetAsString(PatternType) + "|" + (PatternColor == null ? "" : PatternColor.Id) + "|" + (BackgroundColor == null ? "" : BackgroundColor.Id);
            }
        }
        public eDxfFillStyle Style
        {
            get
            {
                return Gradient==null ? eDxfFillStyle.PatternFill : eDxfFillStyle.GradientFill;
            }
            set
            {
                if(value==eDxfFillStyle.PatternFill && Gradient!=null)
                {
                    PatternColor = new ExcelDxfColor(_styles);
                    BackgroundColor = new ExcelDxfColor(_styles);
                    Gradient = null;
                }
                else if(value == eDxfFillStyle.PatternFill && Gradient == null)
                {
                    PatternType = null;
                    PatternColor = null;
                    BackgroundColor = null;
                    Gradient = new ExcelDxfGradientFill(_styles); 
                }
            }
        }
        public ExcelDxfGradientFill Gradient
        {
            get;
            internal set;
        }
        /// <summary>
        /// Creates the the xml node
        /// </summary>
        /// <param name="helper">The xml helper</param>
        /// <param name="path">The X Path</param>
        protected internal override void CreateNodes(XmlHelper helper, string path)
        {
            helper.CreateNode(path);
            if(Style==eDxfFillStyle.PatternFill)
            {
                SetValueEnum(helper, path + "/d:patternFill/@patternType", PatternType);
                SetValueColor(helper, path + "/d:patternFill/d:fgColor", PatternColor);
                SetValueColor(helper, path + "/d:patternFill/d:bgColor", BackgroundColor);
            }
            else
            {
                Gradient.CreateNodes(helper, path);
            }
        }
        /// <summary>
        /// If the object has a value
        /// </summary>
        public override bool HasValue
        {
            get 
            {
                if(Style==eDxfFillStyle.PatternFill)
                {
                    return PatternType != null ||
                            PatternColor.HasValue ||
                            BackgroundColor.HasValue;
                }
                else
                {
                    return Gradient.HasValue;
                }
            }
        }
        public override void Clear()
        {
            if (Style == eDxfFillStyle.PatternFill)
            {
                PatternType = null;
                PatternColor.Clear();
                BackgroundColor.Clear();
            }
            else
            {
                Gradient.Clear();
            }
        }
        /// <summary>
        /// Clone the object
        /// </summary>
        /// <returns>A new instance of the object</returns>
        protected internal override DxfStyleBase Clone()
        {
            return new ExcelDxfFill(_styles) {PatternType=PatternType, PatternColor=(ExcelDxfColor)PatternColor.Clone(), BackgroundColor= (ExcelDxfColor)BackgroundColor.Clone()};
        }
        protected internal override void SetValuesFromXml(XmlHelper helper)
        {
            if (helper.ExistsNode("d:fill/d:patternFill"))
            {
                PatternType = GetPatternTypeEnum(helper.GetXmlNodeString("d:fill/d:patternFill/@patternType"));
                BackgroundColor = GetColor(helper, "d:fill/d:patternFill/d:bgColor/");
                PatternColor = GetColor(helper, "d:fill/d:patternFill/d:fgColor/");
                Gradient = null;
            }
            else if (helper.ExistsNode("d:fill/d:gradientFill"))
            {
                PatternType = null;
                BackgroundColor = null;
                PatternColor = null;
                Gradient = new ExcelDxfGradientFill(_styles);
                Gradient.SetValuesFromXml(helper);
            }
        }
        internal static ExcelFillStyle GetPatternTypeEnum(string patternType)
        {
            if (patternType == "") return ExcelFillStyle.None;
            patternType = patternType.Substring(0, 1).ToUpper(CultureInfo.InvariantCulture) + patternType.Substring(1, patternType.Length - 1);
            try
            {
                return (ExcelFillStyle)Enum.Parse(typeof(ExcelFillStyle), patternType);
            }
            catch
            {
                return ExcelFillStyle.None;
            }
        }
    }
}
