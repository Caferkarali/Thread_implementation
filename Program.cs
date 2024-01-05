using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;

class Program
{
    static ArrayList numbers = new ArrayList();
    static ArrayList primeNumbers = new ArrayList();
    static ArrayList evenNumbers = new ArrayList();
    static ArrayList oddNumbers = new ArrayList();
    static Semaphore semItem = new Semaphore(4, 6); // Başlangıçta 4, maksimum 6 thread çalışabilir bu sayı isteğe göre değişebilir

    static void Main()
    {
        // 1'den 1.000.000'e kadar olan sayıları içeren ArrayList'i oluştur
        for (int i = 1; i <= 1000000; i++)
        {
            numbers.Add(i);
        }

        // 4 eşit parçaya böl
        int parca = numbers.Count / 4;
        ArrayList[] liste = new ArrayList[4];
        for (int i = 0; i < 4; i++)
        {
            liste[i] = new ArrayList(numbers.GetRange(i * parca, parca));
        }

        Stopwatch stopwatch = new Stopwatch();

        // Thread nesnelerini oluştur
        Thread thread1 = new Thread(() => Tek_Cift_Sayi_Bul(liste[0], stopwatch));
        Thread thread2 = new Thread(() => Asal_Bul(liste[1], stopwatch));
        Thread thread3 = new Thread(() => Asal_Bul(liste[2], stopwatch));
        Thread thread4 = new Thread(() => Asal_Bul(liste[3], stopwatch));
        //Threadlere aynı listelerde gönderilebilir
        /* örneğin 
        Thread thread1 = new Thread(() => Tek_Cift_Sayi_Bul(liste[1], stopwatch));
        Thread thread2 = new Thread(() => Asal_Bul(liste[1], stopwatch));
        Thread thread3 = new Thread(() => Asal_Bul(liste[1], stopwatch));
        Thread thread4 = new Thread(() => Asal_Bul(liste[1], stopwatch));
        */

        //döngü ile tüm thradlerin  tüm listlere sıra ile girmesi sağlanabilir


        stopwatch.Start();

        // Thread'leri başlat
        thread1.Start();

        thread2.Start();
        thread3.Start();
        thread4.Start();
        Console.WriteLine("Thread 2, 3  ,4 çalışıyor.");
       

        // Thread'lerin tamamlanmasını bekle
        thread1.Join();
        Console.WriteLine("Thread2 nin asal bulma süresi: ");
        thread2.Join();
        Console.WriteLine("Thread3 nin asal bulma süresi: ");
        thread3.Join();
        Console.WriteLine("Thread4 nin asal bulma süresi: ");
        thread4.Join();

        stopwatch.Stop();

     

 
      
        Console.WriteLine("Ana program bitti.");

        /* Sonuçları yazdır
        Console.WriteLine("Çift Sayılar: " + string.Join(", ", evenNumbers.ToArray()));
        Console.WriteLine("Tek Sayılar: " + string.Join(", ", oddNumbers.ToArray()));
        Console.WriteLine("Asal Sayılar: " + string.Join(", ", primeNumbers.ToArray()));
        */

    }

    static void Tek_Cift_Sayi_Bul(ArrayList sublist, Stopwatch stopwatch)
    {
        Console.WriteLine("Thread 1 çalışıyor.");
        foreach (int num in sublist)
        {
            semItem.WaitOne(); // Semaphore'den izin al
            try
            {
                if (num % 2 == 0)
                    evenNumbers.Add(num);
                else
                    oddNumbers.Add(num);
            }
            finally
            {
                semItem.Release(); // Semaphore'e izni geri ver
            }
        }

        Console.WriteLine("Thread 1 işlem süresi: " + stopwatch.ElapsedMilliseconds + " ms");
    }

    static void Asal_Bul(ArrayList sublist, Stopwatch stopwatch)
    {
       
        foreach (int num in sublist)
        {
            semItem.WaitOne(); // Semaphore'den izin al
            try
            {
                if (IsPrime(num))
                    primeNumbers.Add(num);
            }
            finally
            {
                semItem.Release(); // Semaphore'e izni geri ver
            }
        }

        Console.WriteLine(" " + stopwatch.ElapsedMilliseconds + " ms");
    }

    // Asal sayı kontrolü
    static bool IsPrime(int num)
    {
        if (num < 2)
            return false;

        for (int i = 2; i <= Math.Sqrt(num); i++)
        {
            if (num % i == 0)
                return false;
        }

        return true;
    }
}
