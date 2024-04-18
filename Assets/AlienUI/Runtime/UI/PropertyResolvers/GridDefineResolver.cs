using AlienUI.Models;
using System;
using System.Linq;
using UnityEngine;
using static AlienUI.Models.GridDefine;

namespace AlienUI.PropertyResolvers
{
    public class GridDefineResolver : PropertyResolver
    {
        public override Type GetResolveType()
        {
            return typeof(GridDefine);
        }

        public override object Resolve(string originStr)
        {
            //()|(1*,1*,2*,300px)
            //(1*,1*,2*,300px)|()
            //(1*,1*,2*,300px)|(20px,2*,1*)
            var result = ParseDefines(originStr);

            return new GridDefine(result.Item1, result.Item2);
        }

        public static (Define[], Define[]) ParseDefines(string input)
        {
            var parts = input.Split('|');
            var defines1 = ParseDefinePart(parts[0]);
            var defines2 = ParseDefinePart(parts[1]);
            return (defines1, defines2);
        }

        private static Define[] ParseDefinePart(string part)
        {
            if (part == "()")
            {
                return new Define[1] { new Define { DefineType = EnumDefineType.Weight, Value = 1 } };
            }
            else
            {
                var defines = part.Trim('(', ')').Split(',');
                return defines.Select(d =>
                {
                    var value = float.Parse(d.TrimEnd('*', 'p', 'x'));
                    var defineType = d.EndsWith("*") ? EnumDefineType.Weight : EnumDefineType.Abslute;
                    return new Define { DefineType = defineType, Value = value };
                }).ToArray();
            }
        }
    }
}