# MESNETMD: 2 DIMENSIONAL FRAME ANALYZER WITH MATRIX DISPLACEMENT METHOD #

This is a frame analyzer program that is aimed to solve continuous 2 dimensional ship frames. Since preliminary design of ships in structural manner requires using practical structural analysis programs that do not require detailed information about the structure. Ship frames can be non-uniformed geometries that makes vsrying inertia distibutions on beams, wihch makes the problem even more challenging. The program is desired to have a well-designed graphical user interface to make it easy to use, to implement object oriented programming paradigms to keep the code structured and readable, to use a modern programming application interfaces to maximize utilization of computer resources in efficient way.

## The Theory #

This program uses Matrix Displacement Method with some improvements to the frame. Matrix Displacement Method, also known as the [Direct Stiffness Method](https://en.wikipedia.org/wiki/Direct_stiffness_method), is the most common implementation of the [Finite Element Method](https://en.wikipedia.org/wiki/Finite_element_method) (FEM). Matrix Displacement Method discretizes the system into smaller, basic elements. In this case these basic elements are beam that has 6 degree of freedom.

![Picture beam dof](https://bitbucket.org/repo/dao7ay9/images/2588484908-res1.png)

For each beam element, when all displacements are zero, the forces that are required to create unit displacement for each degree of freedom are obtained and stored into a matrix, which is called the element stiffness matrix.

![Picture stiffness matrix](https://bitbucket.org/repo/dao7ay9/images/1717799172-Screen%20Shot%2004-22-20%20at%2002.50%20PM.PNG)

For each beam element, the forces that are acting on beams are stored in element indirect force vector and the forces that are acting on nodes are stored in element direct force vector. The final element equation will be like the picture below:

![Picture beam equation](https://bitbucket.org/repo/dao7ay9/images/3273377571-Screen%20Shot%2004-22-20%20at%2003.04%20PM.PNG)

This is of course the element equation. In order to solve the problem, it needs to be expressed in the global equation. All forces vectors and the stiffness matrix are express in terms of global axes by multiplying them with transformation matrix. The transformation matrix is:

![Transformation matrix](https://bitbucket.org/repo/dao7ay9/images/4113690622-Screen%20Shot%2004-22-20%20at%2003.24%20PM.PNG)   ![Transformation matrix 2](https://bitbucket.org/repo/dao7ay9/images/2849042738-Screen%20Shot%2004-22-20%20at%2003.21%20PM%20001.PNG)  

where β is the angle of the beam element. 

After that, by using code numbers method, the element stiffness matrices are combined in a global stiffness matrix. Likewise, element force vectors are also combined in global force vector. The global system is then solved to find global displacement vector. After the global solution is done, the displacement vector is imposed in elements equations. The parts of displacement vector in elements are multiplied by element stiffness matrices and the results are summed with indirect force vector to get the force vector. Again, by using transformation matrix, the local force vectors are obtained. After the local force vector is known, bending moment distributions, shear force distributions, axial force distributions and stress distributions can be found.</p> 
