#define training

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace news
{

    class Program
    {

        public static double Distance(List<double> x, List<double> y)
        {
            double r = 0;
            for (int i = 0; i < x.Count; i++)
            {
                r = r + Math.Pow((x[i] - y[i]), 2);
            }
            return Math.Sqrt(r);
        }
        public static double Distance(List<double> x, List<double> y, List<double> weights)
        {
            double r = 0;
            for (int i = 0; i < x.Count; i++)
            {
                r = (double)r + (double)Math.Pow((x[i] - y[i]) * (double)weights[i], 2);
            }
            return (double)Math.Sqrt(r);
        }

        static void Main(string[] args)
        {
            List<List<double>> cat_wrd = new List<List<double>>();                              //количество появлений слов в статьях для каждой категории
            List<List<double>> cat_ttl = new List<List<double>>();                              //количество появлений слов в заголовках для каждой категории
            List<double> word_counter_text = new List<double>();                                //количество появлений каждого слова во всех статьях (в сумме)
            List<double> word_counter_ttl = new List<double>();                                 //количество появлений каждого слова во всех заголовках (в сумме)
            List<String> categories = new List<String>();                                       //список категорий
            List<String> vocabulary = new List<String>();                                       //словарь
            List<List<double>> weights_w = new List<List<double>>();                            //вес слов для статей
            List<List<double>> weights_t = new List<List<double>>();                            //вес слов для заголовков

            int categories_count = 0;                                                           //количество категорий
            int voc_count = 0;                                                                  //количество слов в словаре
            int counter = 0;
            string line;

#if training
            StreamReader train = new StreamReader(@"c:\news\news_train.txt");
            while ((line = train.ReadLine()) != null)
            {
                Console.Clear();
                Console.WriteLine("Training...\n{0}% ({1})", counter / 600, counter);           //процент выполнения, чтоб не так скучно было
                line = line.ToLower();                                                          //перевод строки в нижний регистр
                char[] separators = { '\t' };
                string[] parts = line.Split(separators, StringSplitOptions.RemoveEmptyEntries); //деление строки по символу табуляции
                int cat_index = categories.IndexOf(parts[0]);                                   //поиск категории в списке категорий
                if (cat_index == -1)                                                            //если такой категории нет - 
                {
                    categories.Add(parts[0]);                                                   //добавить новую категорию в список
                    cat_index = categories_count;
                    categories_count++;
                    List<double> t = new List<double>();
                    cat_wrd.Add(t);                                                             //выделить место для новой категории в массивах
                    cat_ttl.Add(t);
                    weights_t.Add(t);
                    weights_w.Add(t);
                    for (int i = 0; i < voc_count; i++)
                    {
                        cat_wrd[cat_index].Add(0);                                              //заполнить нулями
                        cat_ttl[cat_index].Add(0);
                        weights_t[cat_index].Add(0);
                        weights_w[cat_index].Add(0);
                    }
                    
                }
                int parts_length = parts.Length;                                                //количество частей, на которые была разбита строка
                if (parts_length > 1)                                                           //если есть ещё что-то кроме названия категории (иначе была ошибка - выход за пределы массива)
                {                                                                               //Для заголовка:
                    char[] sep = { '.', ',', ' ', ';', '-', '!', '?' };
                    String[] titles;                                                            //список слов в заголовке
                    titles = parts[1].Split(sep, StringSplitOptions.RemoveEmptyEntries);        //составление списка слов в заголовке. В качестве разделителей - элементы массива sep
                    for (int i = 0; i < titles.Length; i++)                                     //удаление всех символов, кроме букв
                    {
                        String temp = "";
                        for (int j = 0; j < titles[i].Length; j++)
                        {
                            if (Char.IsLetter(titles[i], j))
                            {
                                temp += titles[i][j];
                            }
                        }
                        titles[i] = temp;
                    }
                    //Обработка слов
                    for (int i = 0; i < titles.Length; i++)
                    {
                        if (titles[i] != "")                                                    //если строка не пустая
                        {
                            int word_index = vocabulary.IndexOf(titles[i]);                     //нахождение слова в словаре
                            if (word_index == -1)                                               //если слова нет в словаре -
                            {
                                vocabulary.Add(titles[i]);                                      //добавить
                                word_counter_ttl.Add(0);                                        //выделить место в массиве счетчиков
                                word_counter_text.Add(0);
                                word_index = voc_count;
                                voc_count++;                                                    //увеличить счетчик количества слов в словаре
                                for (int k = 0; k < categories_count; k++)
                                {
                                    cat_ttl[k].Add(0);                                          //выделить места под счетчики
                                    cat_wrd[k].Add(0);
                                    weights_w[k].Add(0);
                                    weights_t[k].Add(0);
                                }
                            }
                            cat_ttl[cat_index][word_index]++;                                   //увеличение счетчика появлений слова в заголовках данной категории
                            word_counter_ttl[word_index]++;                                     //увеличение счетчика появлений слова во всех заголовках
                        }
                    }
                    if (parts_length > 2)                                                       //если есть статья
                    {                                                                           //Для статьи:
                        String[] words;                                                         //список слов в статье
                        words = parts[2].Split(sep, StringSplitOptions.RemoveEmptyEntries);     //составление списков слов из статьи. В качестве разделителей - элементы массива sep
                        for (int j = 0; j < words.Length; j++)                                  //удаление всех символов, кроме букв
                        {
                            String temp = "";
                            for (int k = 0; k < words[j].Length; k++)
                            {
                                if (Char.IsLetter(words[j], k))
                                {
                                    temp += words[j][k];
                                }
                            }
                            words[j] = temp;
                        }
                        for (int i = 0; i < words.Length; i++)
                        {
                            if (words[i] != "")                                                 //если строка не пустая
                            {
                                int word_index = vocabulary.IndexOf(words[i]);                  //нахождение слова в словаре
                                if (word_index == -1)                                           //если слова нет в словаре -
                                {
                                    vocabulary.Add(words[i]);                                   //добавить
                                    word_counter_text.Add(0);                                   //выделить место в массиве счетчиков
                                    word_counter_ttl.Add(0);
                                    word_index = voc_count;
                                    voc_count++;                                                //увеличить счетчик количества слов в словаре
                                    for (int k = 0; k < categories_count; k++)
                                    {
                                        cat_wrd[k].Add(0);                                      //выделить места под счетчики
                                        cat_ttl[k].Add(0);
                                        weights_t[k].Add(0);
                                        weights_w[k].Add(0);
                                    }
                                }
                                cat_wrd[cat_index][word_index]++;                               //увеличение счетчика появлений слова в статьях данной категории
                                word_counter_text[word_index]++;                                //увеличение счетчика появлений слова во всех статьях
                            }
                        }
                    }
                }
                counter++;
            }
            train.Close();

            for (int i = 0; i < categories_count; i++)                                          //расчет веса слова (зависит от общеупотребимости)
            {
                for (int j = 0; j < voc_count; j++)
                {
                    double wt, ww;
                    if(word_counter_ttl[j]==0)
                    {
                        wt = 0;
                    }
                    else
                    {
                        wt = (double)(100000D*cat_ttl[i][j]) / ((double)word_counter_ttl[j]);
                    }
                    if(word_counter_text[j]==0)
                    {
                        ww = 0;
                    }
                    else
                    {
                        ww = (double)(100000D*cat_wrd[i][j]) / ((double)word_counter_text[j]);
                    }
                    weights_t[i][j] = wt;                                                       //вес слова определяется как отношение количества появлений слова в статьях (заголовках) данной категории к общему количеству появлений слова в статьях (заголовках), умноженное на 100000
                    weights_w[i][j] = ww;
                }
            }

            counter = 0;

            //Сохранение результатов:

            StreamWriter vectors_out = new StreamWriter(@"c:\news\cat_wrd.txt");                //счетчики слов в категориях для статей
            for (int i = 0; i < categories_count; i++)
            {
                for (int j = 0; j < voc_count; j++)
                {
                    vectors_out.Write("{0} ", cat_wrd[i][j]);
                }
                vectors_out.Write("\n");
            }
            vectors_out.Close();
            vectors_out = new StreamWriter(@"c:\news\cat_ttl.txt");                             //счетчики слов в категориях для заголовков
            for (int i = 0; i < categories_count; i++)
            {
                for (int j = 0; j < voc_count; j++)
                {
                    vectors_out.Write("{0} ", cat_ttl[i][j]);
                }
                vectors_out.Write("\n");
            }
            vectors_out.Close();
            vectors_out = new StreamWriter(@"c:\news\categories.txt");                          //список категорий
            for (int i = 0; i < categories_count; i++)
            {
                vectors_out.WriteLine(categories[i]);
            }
            vectors_out.Close();
            vectors_out = new StreamWriter(@"c:\news\vocabulary.txt");                          //словарь
            for (int i = 0; i < voc_count; i++)
            {
                vectors_out.WriteLine(vocabulary[i]);
            }
            vectors_out.Close();
            vectors_out = new StreamWriter(@"c:\news\word_counter_text.txt");                   //счетчики слов в статьях 
            for (int i = 0; i < voc_count; i++)
            {
                vectors_out.WriteLine(word_counter_text[i]);
            }
            vectors_out.Close();
            vectors_out = new StreamWriter(@"c:\news\word_counter_ttl.txt");                    //счетчики слов в заголовках   
            for (int i = 0; i < voc_count; i++)
            {
                vectors_out.WriteLine(word_counter_ttl[i]);
            }
            vectors_out.Close();
            vectors_out = new StreamWriter(@"c:\news\weights_t.txt");                           //веса для заголовков
            for (int i = 0; i < categories_count; i++)
            {
                for (int j = 0; j < voc_count; j++)
                {
                    vectors_out.Write("{0} ", weights_t[i][j]);
                }
                vectors_out.Write("\n");
            }
            vectors_out.Close();
            vectors_out = new StreamWriter(@"c:\news\weights_w.txt");                           //веса для статей
            for (int i = 0; i < categories_count; i++)
            {
                for (int j = 0; j < voc_count; j++)
                {
                    vectors_out.Write("{0} ", weights_w[i][j]);
                }
                vectors_out.Write("\n");
            }
            vectors_out.Close();
#else
            //Чтение данных из файлов:

            Console.WriteLine("Importing word counters for categories: texts...");              //счетчики слов в категориях для статей
            StreamReader vectors_in = new StreamReader(@"c:\news\cat_wrd.txt");
            while ((line = vectors_in.ReadLine()) != null)
            {
                char[] sp = { ' ' };
                List<double> t = new List<double>();
                cat_wrd.Add(t);
                string[] s_v = line.Split(sp, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < s_v.Length; i++)
                {
                    cat_wrd[counter].Add(double.Parse(s_v[i]));
                }
                counter++;
            }
            vectors_in.Close();
            counter = 0;
            Console.WriteLine("Importing word counters for categories: titles...");             //счетчики слов в категориях для заголовков
            vectors_in = new StreamReader(@"c:\news\cat_ttl.txt");
            while ((line = vectors_in.ReadLine()) != null)
            {
                char[] sp = { ' ' };
                List<double> t = new List<double>();
                cat_ttl.Add(t);
                string[] s_v = line.Split(sp, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < s_v.Length; i++)
                {
                    cat_ttl[counter].Add(double.Parse(s_v[i]));
                }
                counter++;
            }
            vectors_in.Close();
            Console.WriteLine("Importing word counters: texts...");                             //счетчики слов в статьях 
            vectors_in = new StreamReader(@"c:\news\word_counter_text.txt");
            while ((line = vectors_in.ReadLine()) != null)
            {
                word_counter_text.Add(double.Parse(line));
            }
            vectors_in.Close();
            Console.WriteLine("Importing word counters: titles...");                            //счетчики слов в заголовках 
            vectors_in = new StreamReader(@"c:\news\word_counter_ttl.txt");
            while ((line = vectors_in.ReadLine()) != null)
            {
                word_counter_ttl.Add(double.Parse(line));
            }
            vectors_in.Close();
            Console.WriteLine("Importing categories...");                                       //список категорий
            vectors_in = new StreamReader(@"c:\news\categories.txt");
            while ((line = vectors_in.ReadLine()) != null)
            {
                categories.Add(line);
            }
            vectors_in.Close();
            categories_count = categories.Count;
            Console.WriteLine("Importing vocabulary...");                                       //словарь
            vectors_in = new StreamReader(@"c:\news\vocabulary.txt");
            while ((line = vectors_in.ReadLine()) != null)
            {
                vocabulary.Add(line);
            }
            vectors_in.Close();
            counter = 0;
            voc_count = vocabulary.Count;
            Console.WriteLine("Importing weights: texts...");                                   //веса для статей
            vectors_in = new StreamReader(@"c:\news\weights_w.txt");
            while ((line = vectors_in.ReadLine()) != null)
            {
                char[] sp = { ' ' };
                List<double> t = new List<double>();
                weights_w.Add(t);
                string[] s_v = line.Split(sp, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < s_v.Length; i++)
                {
                    weights_w[counter].Add(double.Parse(s_v[i]));
                }
                counter++;
            }
            vectors_in.Close();
            counter = 0;
            Console.WriteLine("Importing weights: titles...");                                  //веса для заголовков
            vectors_in = new StreamReader(@"c:\news\weights_t.txt");
            while ((line = vectors_in.ReadLine()) != null)
            {
                char[] sp = { ' ' };
                List<double> t = new List<double>();
                weights_t.Add(t);
                string[] s_v = line.Split(sp, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < s_v.Length; i++)
                {
                    weights_t[counter].Add(double.Parse(s_v[i]));
                }
                counter++;
            }
            vectors_in.Close();

            Console.WriteLine("Complete.");
#endif

            counter = 0;

            StreamReader data = new StreamReader(@"c:\news\news_test.txt");                     //входные данные
            StreamWriter output = new StreamWriter(@"c:\news\news_out.txt");                    //выходные данные
            while ((line = data.ReadLine())!=null)
            {
                Console.Clear();
                Console.WriteLine("Calculation...\n{0}% ({1})", counter / 150, counter);        //снова уныло смотреть на процент выполнения
                char[] spr = { '\t' };
                char[] sep = { '.', ',', ' ', ';', '-', '?', '!' };
                line = line.ToLower();                                                          //перевод в нижний регистр
                String[] parts = line.Split(spr, StringSplitOptions.RemoveEmptyEntries);        //разделение строки на заголовок и статью
                int parts_length = parts.Length;
                String[] title = parts[0].Split(sep, StringSplitOptions.RemoveEmptyEntries);    //создание списка слов из заголовка
                List<int> ttl_index = new List<int>();
                for (int i = 0; i < title.Length; i++)                                          //удаление всех символов, кроме букв
                {
                    String temp = "";
                    for (int k = 0; k < title[i].Length; k++)
                    {
                        if (Char.IsLetter(title[i], k))
                        {
                            temp += title[i][k];
                        }
                    }
                    title[i] = temp;
                    int ttl_ind = vocabulary.IndexOf(title[i]);
                    if (ttl_ind != -1)                                                          //если слово есть в словаре
                    {
                        ttl_index.Add(ttl_ind);                                                 //добавить его индекс в список
                    }
                }
                List<double> c_ttl = new List<double>();
                int ttl_count = ttl_index.Count;
                for (int i = 0; i < categories_count; i++)
                {
                    c_ttl.Add(0);
                    for (int j = 0; j < ttl_count; j++)
                    {
                        c_ttl[i] = c_ttl[i] + 100000D*Math.Pow((double)weights_t[i][ttl_index[j]]/100000D,2);                      //вычисление суммы весов слов из списка для каждой категории
                    }
                }

                if (parts_length > 1)                                                           //если есть статья
                {
                    String[] words = parts[1].Split(sep, StringSplitOptions.RemoveEmptyEntries);//создание списка слов для статьи
                    List<int> wrd_index = new List<int>();
                    for (int i = 0; i < words.Length; i++)                                      //удаление всех символов, кроме букв
                    {
                        String temp = "";
                        for (int k = 0; k < words[i].Length; k++)
                        {
                            if (Char.IsLetter(words[i], k))
                            {
                                temp += words[i][k];
                            }
                        }
                        words[i] = temp;
                        int wrd_ind = vocabulary.IndexOf(words[i]);
                        if (wrd_ind != -1)                                                      //если слово есть в словаре
                        {
                            wrd_index.Add(wrd_ind);                                             //добавить его индекс в список
                        }
                    }
                    List<double> c_text = new List<double>();
                    int wrd_count = wrd_index.Count;
                    for (int i = 0; i < categories_count; i++)
                    {
                        c_text.Add(0);
                        for (int j = 0; j < wrd_count; j++)
                        {
                            c_text[i] = c_text[i] + 100000D*Math.Pow((double)weights_w[i][wrd_index[j]]/100000D,2);     //вычисление суммы квадратов весов слов из списка для каждой категории
                        }
                        c_ttl[i] = Math.Sqrt(c_text[i]) + Math.Sqrt(c_ttl[i]);                                        //попарное суммирование с весами слов из заголовка
                    }

                }

                int max_index = -1;
                double max = 0;
                for (int i = 0; i < c_ttl.Count; i++)                                           //нахождение наибольшей суммы
                {
                    double c = c_ttl[i];
                    if (c > max)
                    {
                        max = c;
                        max_index = i;
                    }
                }
                if(max_index>-1)
                {
                    output.WriteLine(categories[max_index]);                                    //добавление в файл названия категории с наибольшей суммой
                }
                else
                {
                    output.WriteLine("null");
                }
                counter++;
            }
            data.Close();
            output.Close();

            Console.Clear();
            Console.WriteLine("Complete.");
            Console.ReadKey();
        }
    }
}
