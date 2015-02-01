namespace CorvusAlba.MyLittleLispy.Runtime
{
    public interface IModule
    {
	void Import(Parser parser, Context context);
    }
}