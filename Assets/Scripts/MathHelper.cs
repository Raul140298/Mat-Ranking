public static class MathHelper
{
    public static int GreatestCommonFactor(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    public static int LeastCommonMultiple(int a, int b)
    {
        return (a / GreatestCommonFactor(a, b)) * b;
    }

    public static string SimplifyFractions(int n, int d)
    {
        if (n == 0) return "0";

        int auxn = n, auxd = d;
        int aux = GreatestCommonFactor(n, d);
        if (aux != 1) //they have multiples
        {
            auxn /= aux;
            auxd /= aux;

            if (auxd < 0)
            {
                auxn *= -1;
                auxd *= -1;
            }
        }

        if (auxd == 1)
        {
            return auxn.ToString();
        }
        else
        {
            return auxn.ToString() + " / " + auxd.ToString();
        }
    }
}