# Where Was This Thing Again? Evaluating Methods to Indicate Remembered Object Positions in Virtual Reality

This repository contains the experiment code, data, and analysis for our 2024 manuscript in the *Journal of Vision* (Schuetz, Baltaretu & Fiehler, 2024). 

![Fig1_ResponseMethods](https://github.com/user-attachments/assets/661c85cb-f016-4f23-b883-6e51ea436a2f)


## Abstract

A current focus in sensorimotor research is the study of human perception and action in increasingly naturalistic tasks and visual environments. This is further enabled by the recent commercial success of virtual reality (VR) technology, which allows for highly realistic but well-controlled three-dimensional (3D) scenes. VR enables a multitude of different ways to interact with virtual objects, but only rarely are such interaction techniques evaluated and compared before being selected for a sensorimotor experiment. Here, we compare different response techniques for a memory-guided action task, in which participants indicated the position of a previously seen 3D object in a VR scene: pointing, using a virtual laser pointer of short or unlimited length, and placing, either the target object itself or a generic reference cube. Response techniques differed in availability of 3D object cues and requirement to physically move to the remembered object position by walking. Object placement was the most accurate but slowest due to repeated repositioning. When placing objects, participants tended to match the original object's orientation. In contrast, the laser pointer was fastest but least accurate, with the short pointer showing a good speed–accuracy compromise. Our findings can help researchers in selecting appropriate methods when studying naturalistic visuomotor behavior in virtual environments.


## Experiment Code

Note that the Unity project in the *experiment/* subfolder documents the scripts and custom scenes used to implement the pointing and placement tasks described in the paper, but is not meant to provide an "click and run" experiment. In particular, the scene and object assets used in the study are not included, both because they were provided to us by the [Scene Grammar Lab team at Goethe University Frankfurt](https://www.scenegrammarlab.com/research/) (many thanks!), but also because they would have grown the size of this repository by an order of magnitude. 

Researchers interested in replicating this work or using this codebase to build their own study are encouraged to reach out to the first author, who will be able to share the (much larger) full Unity project on a case by case basis. 


## Citation

If you use any data or code from this repository, please cite the corresponding manuscript: 

*Schuetz, I., Baltaretu, B. R., & Fiehler, K. (2024). Where was this thing again? Evaluating methods to indicate remembered object positions in virtual reality. Journal of Vision, 24(7), 10-10. https://doi.org/10.1167/jov.24.7.10.*

Citation in BibTeX format:

```
@article{10.1167/jov.24.7.10,
    author = {Schuetz, Immo and Baltaretu, Bianca R. and Fiehler, Katja},
    title = "{Where was this thing again? Evaluating methods to indicate remembered object positions in virtual reality}",
    journal = {Journal of Vision},
    volume = {24},
    number = {7},
    pages = {10-10},
    year = {2024},
    month = {07},
    issn = {1534-7362},
    doi = {10.1167/jov.24.7.10},
    url = {https://doi.org/10.1167/jov.24.7.10},
    eprint = {https://arvojournals.org/arvo/content\_public/journal/jov/938681/i1534-7362-24-7-10\_1720702858.31483.pdf},
}
```
