using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if DEBUG
using System.Diagnostics;
#endif

namespace TileGame
{
    public struct Vector2D
    {
        public Int32 X;
        public Int32 Y;

        public Vector2D(Int32 aX,Int32 aY)
        {
            X = aX;
            Y = aY;
        }
    }

    public enum Direction
    {
        Left,
        Right,
        Up,
        Down,

        None
    }

    public class Tile
    {
        private const Int32 BLANK_TILE = -1;
        private List<Int32> mContent;
        private Int32 mWidth;
        private Int32 mHeight;
        private Int32 mMaxFlags;
        private Int32 mLength;
        private Vector2D mBlank;

        public Tile(Int32 aWidth, Int32 aHeight)
        {
            mLength = aWidth * aHeight;
            mContent = new List<Int32>(mLength);
            for (Int32 i = 0; i < mLength - 1; i++)
            {
                mContent.Add(i);
            }
            mContent.Add(BLANK_TILE);
            mWidth = aWidth;
            mHeight = aHeight;
            mMaxFlags = mLength - 2;
            mBlank = new Vector2D(aWidth, aHeight);
        }

        public Int32 Width
        {
            get { return mWidth; }
        }

        public Int32 Height
        {
            get { return mHeight; }
        }

        public void Shuffle()
        {
            Random rand = new Random();
            for (Int32 i = 0; i < mLength; i++)
            {
                Int32 dst = rand.Next(mLength);
                Int32 tmp = mContent[i];
                mContent[i] = mContent[dst];
                mContent[dst] = tmp;
            }

            for (Int32 y =0; y < Height; y++)
            {
                for (Int32 x = 0; x < Width; x++)
                {
                    if (mContent[y * Width + x] == BLANK_TILE)
                    {
                        mBlank.X = x + 1;
                        mBlank.Y = y + 1;
                        break;
                    }
                }
            }

        }

        public bool HandleClick(Int32 aX, Int32 aY)
        {
            if (ValidClick(aX, aY) != Direction.None)
            {
#if DEBUG
                Debug.WriteLine("####### YES,CLICK RIGHT ########");
#endif
                Int32 i = (aY - 1) * Width + aX - 1;
                Int32 j = (Blank.Y - 1) * Width + Blank.X - 1;
                Int32 tmp = mContent[i];
                mContent[i] = mContent[j];
                mContent[j] = tmp;
                Blank = new Vector2D(aX, aY);
                return true;
            }
#if DEBUG
            else
            {
                Debug.WriteLine("------- NO,TRY AGAIN ---------");
            }
#endif
            return false;
        }

        public bool CheckFinish()
        {
            for (int i = 0; i < mLength - 1; i++)
            {
                if (mContent[i] != i)
                {
                    return false;
                }
            }
            return true;
        }

        public Direction ValidClick(Int32 aX, Int32 aY)
        {
            if (aX > Width || aX < 1)
            {
                return Direction.None;
            }
            if (aY > Height || aY < 1)
            {
                return Direction.None;
            }
            if (aX == Blank.X)
            {
                if(aY == Blank.Y - 1)
                {
                    return Direction.Up;
                }
                else if(aY == Blank.Y + 1)
                {
                    return Direction.Down;
                }
            }
            if (aY == Blank.Y)
            {
                if(aX == Blank.X - 1)
                {
                    return Direction.Left;
                }
                else if (aX == Blank.X + 1)
                {
                    return Direction.Right;
                }
            }
            return Direction.None; ;
        }

        public void Print()
        {
            for (Int32 i = 0; i < mLength; i++)
            {
                if (i % Width == 0)
                {
                    Console.WriteLine();
                }
                else
                {
                    Console.Write("   ");
                }
                Console.Write(mContent[i]);
            }
            Console.WriteLine("\nBlank at:" + mBlank.X.ToString() + " " + mBlank.Y.ToString());
        }

        public Vector2D Blank
        {
            get { return mBlank; }
            private set { mBlank = value; }
        }

        public Int32 this[Int32 index]
        {
            get { return mContent[index]; }
        }
    }
}
