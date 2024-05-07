using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlienUI.Models
{
    public struct ConditionSets
    {
        public List<Condition> Conditions;

        public ConditionSets(List<Condition> conditions)
        {
            Conditions = conditions;
        }

        public readonly bool Test(object value)
        {
            if (Conditions == null) return false;

            foreach (var con in Conditions)
            {
                if (!con.Test(value))
                    return false;
            }

            return true;
        }
    }

    public struct Condition
    {
        public string CompareValueStr;
        public object CompareValue;
        public EnumCompareType CompareType;

        public Condition(string compareValue, EnumCompareType compareType)
        {
            CompareValueStr = compareValue;
            CompareType = compareType;
            CompareValue = null;
        }

        public Condition ResolveCompareValue(Engine eng, Type valueType)
        {
            var t = eng.GetAttributeResolver(valueType);
            CompareValue = t?.Resolve(CompareValueStr);

            return this;
        }

        public readonly bool Test(object value)
        {
            if (CompareValue == null) return false;

            var result = CompareType switch
            {
                EnumCompareType.Equal => object.Equals(value, CompareValue),
                EnumCompareType.GreaterThan when value is IComparable comValue => comValue.CompareTo(CompareValue) > 0,
                EnumCompareType.GreaterThanOrEqual when value is IComparable comValue => comValue.CompareTo(CompareValue) >= 0,
                EnumCompareType.LessThan when value is IComparable comValue => comValue.CompareTo(CompareValue) < 0,
                EnumCompareType.LessTranOrEqual when value is IComparable comValue => comValue.CompareTo(CompareValue) <= 0,
                _ => false
            };

            return result;
        }
    }

    public enum EnumCompareType
    {
        Equal, GreaterThan, GreaterThanOrEqual, LessThan, LessTranOrEqual
    }
}
