using AlienUI.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace AlienUI
{
    public class ConditionResolver : PropertyResolver<ConditionSets>
    {
        static Regex reg = new Regex(@"^(==|>|>=|<|<=)(\w+)$");
        protected override ConditionSets OnResolve(string originStr)
        {
            List<Condition> conditions = new List<Condition>();
            var temp = originStr.Split("&&");
            foreach (var str in temp)
            {
                var match = reg.Match(str);
                if (!match.Success) return default;

                EnumCompareType ct = match.Groups[1].Value switch
                {
                    "==" => EnumCompareType.Equal,
                    ">" => EnumCompareType.GreaterThan,
                    "<" => EnumCompareType.LessThan,
                    ">=" => EnumCompareType.GreaterThanOrEqual,
                    "<=" => EnumCompareType.LessTranOrEqual,
                    _ => default
                };

                Condition con = new Condition(match.Groups[2].Value, ct);
                conditions.Add(con);
            }

            ConditionSets result = new ConditionSets(conditions);

            return result;
        }

        protected override ConditionSets OnLerp(ConditionSets from, ConditionSets to, float progress)
        {
            if (progress >= 0) return to;
            else return from;
        }
    }
}
