namespace Gui
{
    internal class Fragment
    {
        public Fragment()
        {
        }

        public Triangle PixelTriangle { get; set; }
        public Triangle SourceTriangle { get; internal set; }
    }
}