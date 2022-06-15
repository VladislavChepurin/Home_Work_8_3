using System.Text;

namespace HomeWork_8_3;

class Program
{
    static void Main(string[] args)
    {
        // Входящая папка Volume размещена на рабочем столе на системных папках лучше не 
        string directory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Volume";

        //Врядли метод будет запускаться на большом количестве файлов, следовательно бех асинхрона
        Console.WriteLine($"Исходный размер папки: {GetVolume(directory)}");

        Console.WriteLine($"Освобождено: {Clear(directory)}");

        //Тут два пути либо вычесть то что вернул Clear, либо заново запросить
        //Вычитание быстрее, запрос точнее, т.к. в ТЗ нет указания выбираю точнее
        Console.WriteLine($"Текущий размер папки: {GetVolume(directory)}");
    }

    static long Clear(string directory)
    {
        var timeDelts = DateTime.Now - TimeSpan.FromMinutes(3);
        long size = default;

        // Проверяем наличие директории
        if (Directory.Exists(directory) && FullFoolproof(directory))
        {
            DirectoryInfo info = new(directory);
            try
            {
                // Иррретация по файлам
                foreach (var itemFiles in info.GetFiles())
                {
                    if (itemFiles.LastAccessTime < timeDelts)
                    {
                        FileInfo file = new FileInfo(itemFiles.ToString());
                        size += file.Length;
                        file.Delete();
                    }
                }
                // Иррретация по директориям
                foreach (var itemDir in info.GetDirectories())
                {
                    if (itemDir.LastAccessTime < timeDelts)
                    {
                        DirectoryInfo deleteDir = new DirectoryInfo(itemDir.ToString());
                        size += GetVolume(itemDir.ToString());
                        deleteDir.Delete(true);
                    }
                }
            }

            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine($"Произошла ошибка, проверьте атрибуты файлов, возможно они помечены как \"только для чтения\"  {Environment.NewLine} {e.Message}");
            }
            catch (IOException e)
            {
                Console.WriteLine($"Произошла ошибка, проверьте возможно файлы использовались другой программой. {Environment.NewLine} {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Произошла ошибка, {Environment.NewLine} {e.Message}");
            }
        }
        else
        {
            Console.WriteLine($"Указанной папки не существует, или папка системная, или запрещенная к работе с этой программой");
        }
        return size;
    }

    static Boolean FullFoolproof(string dir)
    {
        if ((dir == Environment.GetFolderPath(Environment.SpecialFolder.Windows) ||
             dir == Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) ||
             dir == Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) ||
             dir == Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) ||
             dir == Environment.GetFolderPath(Environment.SpecialFolder.Desktop)))
        {
            return false;
        }
        return true;
    }

    static long GetVolume(string directory, long size = 0)
    {
        // Проверяем наличие папки
        if (Directory.Exists(directory))
        {
            try
            {
                DirectoryInfo infoDir = new(directory);
                foreach (var itemFiles in infoDir.GetFiles())
                {
                    FileInfo info = new(itemFiles.ToString());
                    size += info.Length;
                }
                foreach (var itemDir in infoDir.GetDirectories())
                {
                    size = GetVolume(itemDir.ToString(), size);
                }
            }

            catch (Exception e)
            {
                Console.WriteLine($"Произошла ошибка, {Environment.NewLine} {e.Message}");
            }
        }
        else
        {
            Console.WriteLine($"Указанной папки не существует");
        }
        return size;
    }
}