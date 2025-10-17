/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ec.edu.monster.view;

/**
 *
 * @author ednan
 */

import ec.edu.monster.view.ConsoleView;
import ec.edu.monster.controller.CliController;

public class CliMain {
    public static void main(String[] args){
        ConsoleView view = new ConsoleView();
        CliController controller = new CliController(view);
        controller.iniciar();
    }
}
