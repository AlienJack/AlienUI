using UnityEngine;

namespace AlienUI.Models
{
    public class GridDefine
    {
        public static GridDefine Default => new GridDefine(new Define[] { new Define { DefineType = EnumDefineType.Weight, Value = 1 } }, new Define[] { });

        public int Column => m_columnDef.Length;
        public int Row => m_rowDef.Length;

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
            m_cellWidth = new float[Column];
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

            m_cellHeight = new float[Row];
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

            m_cellOffsetX = new float[Column];
            float offset = 0;
            for (int i = 0; i < Column; i++)
            {
                m_cellOffsetX[i] = offset;
                offset += m_cellWidth[i];
            }
            m_cellOffsetY = new float[Row];
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
