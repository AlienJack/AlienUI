using AlienUI.Models;
using System.Linq;
using System.Text;
using UnityEngine;
using static AlienUI.Models.GridDefine;

namespace AlienUI.PropertyResolvers
{
    public class GridDefineResolver : PropertyResolver<GridDefine>
    {
        protected override GridDefine OnResolve(string originStr)
        {
            //()|(1*,1*,2*,300px)
            //(1*,1*,2*,300px)|()
            //(1*,1*,2*,300px)|(20px,2*,1*)
            var result = ParseDefines(originStr);

            return new GridDefine(result.Item1, result.Item2);
        }


        protected override GridDefine OnLerp(GridDefine from, GridDefine to, float progress)
        {
            if (Mathf.Approximately(progress, 1f))
                return to;
            else
                return from;
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

        protected override string Reverse(GridDefine value)
        {
            value.GetDefines(out var cols, out var rows);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < cols.Count; i++)
            {
                var item = cols[i];
                if (item.DefineType == EnumDefineType.Abslute)
                    sb.Append($"{item.Value}px");
                else if (item.DefineType == EnumDefineType.Weight)
                    sb.Append($"{item.Value}*");

                if (i < cols.Count - 1)
                    sb.Append(',');
            }
            sb.Append('|');
            for (int i = 0; i < rows.Count; i++)
            {
                var item = rows[i];
                if (item.DefineType == EnumDefineType.Abslute)
                    sb.Append($"{item.Value}px");
                else if (item.DefineType == EnumDefineType.Weight)
                    sb.Append($"{item.Value}*");

                if (i < rows.Count - 1)
                    sb.Append(',');
            }

            return sb.ToString();
        }
    }
}