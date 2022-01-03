[![DOI](https://zenodo.org/badge/443986666.svg)](https://zenodo.org/badge/latestdoi/443986666)

# Introduction

This code implements a simulation of a smart service in a production system in order to estimate its impact on KPIs. To limit the effort, the production system is modelled as a three stage process: The first stage aggregates all real processes taking place before the smart service, the second stage is the process influenced by the smart service, and the third stage sums up all processes happening after the smart service.

<h1 align="center">
    <a href="https://github.com/Tools-for-Production-Science/dynamic-pricing-in-production-networks" title="Smart Service Simulation">
    <img width=90% alt="" src="https://github.com/Tools-for-Production-Science/smart-service-in-production-simulation/blob/master/Concept.jpg"> </a>
    <br>
</h1>

# Example
A manfucaturer wants to estimate the impact of a predictive maintenance system for a CNC machine on the over all production performance to decides wheather it could be a valuable service for the company. The manufacturer models all processes like purchase, inventory, preparation etc as the first stage process. The CNC process is the main process. The impact of the smart service on the main process is estimated, e.g. how much down time can be saved. Process steps after the tooling like packaging and delivery are aggregated as the third stage. The simulation is fed with data about the arrival of customer orders, order prices and cost etc. After the simulation run a distribution of the outcome of different KPIs can be evaluated. 

<h1 align="center">
    <a href="https://github.com/Tools-for-Production-Science/dynamic-pricing-in-production-networks" title="Smart Service Simulation">
    <img width=90% alt="" src="https://github.com/Tools-for-Production-Science/smart-service-in-production-simulation/blob/master/Example.png"> </a>
    <br>
</h1>

# Target groups of this work

- Other researcher in the field of production sciences
- Practitioners who want to estimate the profitability of a smart service early on

# What is special about this work

- clear focus on low setup effort
- Fast estimation of the smart service effect
- Based on Sim# (C#)
