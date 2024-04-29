using UnityEngine;

namespace AlienUI.Models
{
    public struct GridDefine
    {
        public static GridDefine Default => new GridDefine(new Define[] { new Define { DefineType = EnumDefineType.Weight, Value = 1 } }, new Define[] { });

        public readonly int Column => m_columnDef.Length;
        public readonly int Row => m_rowDef.Length;

        private Define[] m_columnDef;
        private Define[] m_rowDef;

        private float[] m_cellWidth;
        private float[] m_cellHeight;
        private float[] m_cellOffsetX;
        private float[] m_cellOffsetY;

        public GridDefine(Define[] columnDefine, Define[] rowDefine)
        {
            m_columnDef = columnDefine;
            m_rowDef = rowDefine;
            m_cellWidth = new float[m_columnDef.Length];
            m_cellHeight = new float[m_rowDef.Length];
            m_cellOffsetX = new float[m_columnDef.Length];
            m_cellOffsetY = new float[m_rowDef.Length];
        }

        public Float2 GetCellSize(int column, int row)
        {
            Float2 size = default;
            size.x = m_cellWidth.Length <= column ? 0 : m_cellWidth[column];
            size.y = m_cellHeight.Length <= row ? 0 : m_cellHeight[row];

            return size;
        }

        public Float2 GetCellOffset(int column, int row)
        {
            Float2 offset = default;
            column = Mathf.Min(m_cellOffsetX.Length - 1, column);
            row = Mathf.Min(m_cellOffsetY.Length - 1, row);
            offset.x = m_cellOffsetX[column];
            offset.y = -m_cellOffsetY[row];
            return offset;
        }

        public void CalcCellSizes(float totalWidth, float totalHeight)
        {
            //计算宽度
            {
                float totalWeight = 0;
                //先计算绝对模式的宽度,同时收集权重总值
                for (int i = 0; i < Column; i++)
                {
                    var type = m_columnDef[i].DefineType;
                    var value = m_columnDef[i].Value;

                    if (type == EnumDefineType.Abslute)
                    {
                        m_cellWidth[i] = Mathf.Min(totalWidth, value);
                        totalWidth -= m_cellWidth[i];
                    }
                    else
                    {
                        totalWeight += value;
                    }
                }

                //再计算权重模式宽度
                for (int i = 0; i < Column; i++)
                {
                    var type = m_columnDef[i].DefineType;
                    var value = m_columnDef[i].Value;

                    if (type == EnumDefineType.Weight)
                    {
                        m_cellWidth[i] = totalWidth * (value / totalWeight);
                    }
                }
            }

            //计算高度
            {
                float totalWeight = 0;
                //先计算绝对模式的高度,同时收集权重总值
                for (int i = 0; i < Row; i++)
                {
                    var type = m_rowDef[i].DefineType;
                    var value = m_rowDef[i].Value;

                    if (type == EnumDefineType.Abslute)
                    {
                        m_cellHeight[i] = Mathf.Min(totalHeight, value);
                        totalHeight -= m_cellHeight[i];
                    }
                    else
                    {
                        totalWeight += value;
                    }
                }

                //再计算权重模式高度
                for (int i = 0; i < Row; i++)
                {
                    var type = m_rowDef[i].DefineType;
                    var value = m_rowDef[i].Value;

                    if (type == EnumDefineType.Weight)
                    {
                        m_cellHeight[i] = totalHeight * (value / totalWeight);
                    }
                }
            }

            float offset = 0;
            for (int i = 0; i < Column; i++)
            {
                m_cellOffsetX[i] = offset;
                offset += m_cellWidth[i];
            }
            offset = 0;
            for (int i = 0; i < Row; i++)
            {
                m_cellOffsetY[i] = offset;
                offset += m_cellHeight[i];
            }
        }

        public enum EnumDefineType
        {
            /// <summary> 绝对值 </summary>
            Abslute,
            /// <summary> 权重值 </summary>
            Weight,
        }

        public struct Define
        {
            public EnumDefineType DefineType;
            public float Value;
        }
    }
}
