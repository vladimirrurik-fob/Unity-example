namespace Code.Infrastructure.Services.SaveLoad
{
   //ISaveLoad - Будем помечать сущности, который нам нужно сохранить.
   //SaveLoadRegistry - класс, который будет собирать и регистрировать ISaveLoad сущности
   //ProgressService - сервис, который будет держать в себе большой композитный Data файл для сохранений.
   //SaveLoadService - сервис, который будет хранить в себе SLRegistry, и будут методы Save(), Load(), Reset();
}