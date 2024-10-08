﻿using System.ComponentModel;
using System.Reflection;

namespace Boxy_Core.Utilities
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// If an Enum value has a description attribute, that will be returned.
        /// Otherwise the regular ToString() will be used.
        /// </summary>
        /// <param name="en">Enum to get description from.</param>
        /// <returns>Description attribute as a string if it exists, otherwise ToString().</returns>
        public static string ToDescription(this Enum en)
        {
            if (en == null)
            {
                return string.Empty;
            }

            // This should always work, since we are getting the type from the enum, and then
            // asking for the member which matches the enum as a string.
            MemberInfo memInfo = en.GetType().GetMember(en.ToString()).Single();

            // This will be null if no description attribute is attached to this enum.
            DescriptionAttribute? descriptionAttribute = memInfo.GetCustomAttributes(typeof(DescriptionAttribute), false).OfType<DescriptionAttribute>().FirstOrDefault();
            return descriptionAttribute != null ? descriptionAttribute.Description : en.ToString();
        }

        /// <summary>
        /// Converts the enum to the actual width of the line.
        /// </summary>
        public static double ToPointSize(this CutLineSizes cutLineSize)
        {
            switch (cutLineSize)
            {
                case CutLineSizes.Small:
                    return 0.5;
                case CutLineSizes.Medium:
                    return 1;
                case CutLineSizes.Large:
                    return 1.5;
                case CutLineSizes.QuiteLarge:
                    return 3;
                case CutLineSizes.Colossal:
                    return 5;
                case CutLineSizes.ALineToSurpassMetalGear:
                    return 16;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cutLineSize), cutLineSize, "Enum value not handled in switch.");
            }
        }
    }
}
