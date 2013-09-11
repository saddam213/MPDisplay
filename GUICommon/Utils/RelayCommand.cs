using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MPDisplay.Common.Utils
{
    /// <summary>
    /// 
    /// </summary>
   public class RelayCommand : ICommand
   {
       #region Fields 

       readonly Action<object> _execute;
       private readonly Func<bool> _canExecute;

       #endregion 
       
       #region Constructors 

       /// <summary>
       /// Initializes a new instance of the <see cref="RelayCommand"/> class.
       /// </summary>
       /// <param name="execute">The execute.</param>
       public RelayCommand(Action execute) : this(execute, null) { }

       public RelayCommand(Action<object> execute) : this(execute, null) { }

       /// <summary>
       /// Initializes a new instance of the <see cref="RelayCommand"/> class.
       /// </summary>
       /// <param name="execute">The action to execute.</param>
       /// <param name="canExecute">The can execute.</param>
       /// <exception cref="System.ArgumentNullException">execute</exception>
       public RelayCommand(Action execute, Func<bool> canExecute) 
       { 
           if (execute == null)
               throw new ArgumentNullException("execute");
           
           _execute = p => execute();
           _canExecute = canExecute;
       }

       public RelayCommand(Action<object> execute, Func<bool> canExecute)
       {
           if (execute == null)
               throw new ArgumentNullException("execute");

           _execute = execute;
           _canExecute = canExecute;
       } 

       #endregion 

       #region ICommand Members 

       /// <summary>
       /// Defines the method that determines whether the command can execute in its current state.
       /// </summary>
       /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
       /// <returns>
       /// true if this command can be executed; otherwise, false.
       /// </returns>
       [DebuggerStepThrough]
       public bool CanExecute(object parameter)
       {
           return _canExecute == null ? true : _canExecute();
       }

       /// <summary>
       /// Occurs when changes occur that affect whether or not the command should execute.
       /// </summary>
       public event EventHandler CanExecuteChanged 
       { 
           add { CommandManager.RequerySuggested += value; } 
           remove { CommandManager.RequerySuggested -= value; } 
       }

       /// <summary>
       /// Defines the method to be called when the command is invoked.
       /// </summary>
       /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
       public void Execute(object parameter)
       {
           _execute(parameter); 
       } 
       
       #endregion 
   }

   public class RelayCommand<T> : ICommand
   {
       #region Fields

       readonly Action<T> _execute;
       readonly Predicate<T> _canExecute;

       #endregion

       #region Constructors

       /// <summary>
       /// Initializes a new instance of the <see cref="RelayCommand"/> class.
       /// </summary>
       /// <param name="execute">The execute.</param>
       public RelayCommand(Action<T> execute) : this(execute, null) { }

       /// <summary>
       /// Initializes a new instance of the <see cref="RelayCommand"/> class.
       /// </summary>
       /// <param name="execute">The action to execute.</param>
       /// <param name="canExecute">The can execute.</param>
       /// <exception cref="System.ArgumentNullException">execute</exception>
       public RelayCommand(Action<T> execute, Predicate<T> canExecute)
       {
           if (execute == null)
               throw new ArgumentNullException("execute");

           _execute = execute;
           _canExecute = canExecute;
       }

       #endregion

       #region ICommand Members

       /// <summary>
       /// Defines the method that determines whether the command can execute in its current state.
       /// </summary>
       /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
       /// <returns>
       /// true if this command can be executed; otherwise, false.
       /// </returns>
       [DebuggerStepThrough]
       public bool CanExecute(object parameter)
       {
           if (_canExecute == null)
           {
               return true;
           }

           return parameter == null ? false : _canExecute((T)parameter);
       }

       /// <summary>
       /// Occurs when changes occur that affect whether or not the command should execute.
       /// </summary>
       public event EventHandler CanExecuteChanged
       {
           add { CommandManager.RequerySuggested += value; }
           remove { CommandManager.RequerySuggested -= value; }
       }

       /// <summary>
       /// Defines the method to be called when the command is invoked.
       /// </summary>
       /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
       public void Execute(object parameter)
       {
           _execute((T)parameter);
       }

       #endregion
   }


  
}
