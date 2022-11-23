# Introduction

In the *Game of Homes* you need to create and implement the best 30-year-policy for a housing association, which has a number of long-term goals. The sustainability of your policy depends on how successful you are in fulfilling these goals.

# Getting started

## Requirements

Windows 10, Version 1809 or newer.
MacOS 10.13 "High Sierra" or newer.
Various Linux distributions.

## Installation

Download and install [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) for either Windows, MacOS or Linux. `Make sure that you pick the correct version`like x64 otherwise it will not run. It is unlikely that you need x86 since all CPU's nowadays are 64 bit (=`x64`).
For more details which OS is supported and how to install see https://docs.microsoft.com/en-us/dotnet/core/install/.
Download and install [Visual Studio Code](https://code.visualstudio.com/download) for either Windows, MacOS or Linux.

## Running the dashboard

Start Visual Studio Code, go to menu `View --> Extensions` and search for C#. Choose `C# for Visual Studio Code (Microsoft)`.

Download the source files. They are packed in a zip archive, extract the files to a local folder.

In visual studio code, go to menu `File --> Open Folder`. Navigate to the `Source`folder that you have just extracted from the zip file. This last step is important otherwise paths are wrong and you cannot run the program. If visual studio asks for  installing  missing  assets  then choose Yes.

In the bottom there is window with a tab `Terminal`. If not, go to menu `Terminal --> New Terminal`.
Run the following statement:


Windows: `dotnet watch --project .\GoHDashboard\Blazor-Dashboard run`
Linux or macOS:`dotnet watch --project ./GoHDashboard/Blazor-Dashboard run`


A browser window pops up with page [https://localhost:5001](https://localhost:5001), if not please navigate to this address yourself. The page shows the dashboard that presents the results. It is a good idea to open the page in incognito / private mode, because sometime the page is cached and new results do appear only after clearing your browsers cache.

The page refreshes itself during the assignment so that you see how the modifications pan out.

If something is not working it can be that you need to either `restart Visual Studio Code`, or even `reboot your machine`. The above  dotnet  command restores everything, compiles the dashboard project and starts a listener automatically. If it does not restore, run

`dotnet restore`

## Running the game of homes simulation

Just press F5 or go to menu `Run --> Start debugging`. If required, choose the game of homes calculator. This starts a simulation and some output is printed to the terminal. If finished, head over to the browser. The new results will be shown. If not, please refresh the page.
Sometimes the results do not change, in that case you have to clear the cache of the browser. How to do this depends on the browser that you are using which can be easily found by looking on the internet. Of just open the page in incognito mode.

## Goals

The following goals are minimum requirements. Failing to meet them has severe consequences for the housing association. In order of importance:

1. **Solvency ratio must not drop below 20%**, as this equals bankruptcy. For housing associations the solvency ratio is the equity (=total assets - loans) divided by the total assets (=real estate value + cash). In other words, the part of your total assets that is NOT financed by taking loans. It must be at least 20% at all times or the housing association is declared bankrupt. Of course, your policy should result in a very low risk for bankruptcy.

2. **Have at least 180 houses in total and have at least 50 houses with a low rent**  (less than € 400 after adjusting for inflation) .  Those goals are agreed with the municipality, in order to have sufficient housing available within the municipality. 


Taking the above requirements in account, the housing association has the following general goals:

3. **High sustainability of owned houses**. All houses have a sustainability rating which indicates the CO2 efficiency of the house. The housing association wants to minimize the amount of houses with a sustainability rating below 30. A higher rating is always better, but above 70 an increase is less important than between 30 and 70.
4. **High market share**. The housing association has agreed with the municipality to rent at least 180 houses. However, they strive to a higher market share. The board thinks that a number higher than 250 does not give extra benefits.
5. **Affordable rents**. In general, the housing association strives to affordable rents. A lower actual rent per house (compared to the market rent) is preferred.

## Score

A scoring system rates any policy. For 100 economic scenarios it calculates a scenario score, showing how well the policy fulfills the goals in that scenario. All goals are taken into account in various degrees, but if the housing association goes bankrupt, the scenario score is 0 by definition. The policy score is the average of the 100 scenario scores.

## Starting policy

The starting point of this case is a policy that never raises the rent, never sells houses and  never develops new projects. As you can imagine, it does not score very well, because in almost every scenario this results in bankruptcy as the maintenance costs increase but the rent does not. Your goal is to improve the policy as much as you can to achieve the highest score.

There will be three rounds. In the first round you can only adjust the rent strategy, in the second you may also adjust the selling strategy and in the third round you may also change the investment strategy. During every round you may try as many ideas as you like.

## Model

The policy is implemented in `PolicyImplementation.cs`. The rent, selling and investment strategies each have a function. The comments explain how policy decisions can be implemented. You can (and should!) use information from the following classes:

* `Economy`, for information on the current economic scenario, for example the current interest rate.

* `House`, for information on a particular house, for example the sustainability. 

* `HousingAssociation`, for information on how well (or how bad) the housing association is currently doing. 

Examples are in the comments above the functions in `PolicyImplementation.cs`.

# Round 1: Improve the rent strategy

A low rent is important for the housing association, but costs increase over time, so rents must increase as well to avoid bankruptcy. There are two ways a housing association can increase rents over time. As long as the current tenant (*huurder*) does not leave the house, the rent can be increased with (at most) inflation each year. However, when a new tenant enters the house, a higher rent can be asked. The maximum rent which can be asked to the new tenant is called the maximum allowed rent.

In the following to steps, determine a rent strategy that results in low rents without endangering the solvency ratio of the housing association. 

In the starting policy, the rent of the new year is equal to the rent of the previous year.

## Step 1a: Simple increase of the rent

Construct a new rent definition in which the previous rent is always increased by inlation. Implement this new definition in function `DetermineNewRent ` and test it. 

## Step 1b: Ask the maximum allowed rent for new tenants

When a new tenant enters the house, a higher rent can be asked. The maximum rent which can be asked is called the maximum allowed rent. Adjust the function `DetermineNewRent` to ask this rent when the current tenant leaves. 

## Step 1c: Also take the financial position of the housing association into account

If you know how well the housing association is doing, you can make a ‘smart’ policy f.e. increase the rents if the housing association risks bankruptcy. If it is doing fine the increase can be less severe or even negative, resulting in lower rents. Implement this in the same function and finetune it to get a high score.

# Round 2: Introduce a selling strategy

A housing association can improve its cash position (and hence the solvency ratio) by selling houses. Empty houses are worth more than occupied houses, so it would be optimal to sell a house when a tenant has just left the house. Selling houses can also be a method to get rid of low-sustainable houses, but they are typically worth less than high-sustainable houses.

Improve your policy in function `DetermineSell`. The function is called for every year. You can use the various characteristics of the houses to decide whether to sell the house or not in a certain year. If you decide not to sell in a certain year, you can still sell it the next years.

You may still change the rent strategy, as a new selling strategy may necessitate a different rent strategy.

# Round 3: Introduce an investment strategy

The money that was earned with selling houses can also be invested by the housing association. Investments can be made in new houses or existing houses.

## Step 3a: Invest in new houses

The housing association may decide to invest in building new houses. This results in a higher number of owned houses (which is a goal). New houses usually have a high initial sustainability, which is also good.

Your policy can decide to invest in up to 5 construction projects. Every project can be done at most once.

| **Project name**        | **Number of houses** | **Construction costs ** | Market value if empty | **Initial maximum allowed rent** | **Maintenance costs** | **Sustainability** |
| ----------------------- | -------------------: | ------------------: | --------------------: | -----------------------: | --------------------: | ------------------------: |
| **Student Flat**        |                   25 |              70,000 |                80,000 |							 400 |                   250 |                        35 |
| **Zero emission house** |                    5 |             300,000 |               320,000 |						  1,400 |                   500 |                        95 |
| **Flat**                |                   20 |             110,000 |               120,000 |						    550 |                   250 |                        60 |
| **Terraced house**      |                   15 |             120,000 |               125,000 |						     600 |                   300 |                        55 |
| **Semi-detached house** |                   10 |             180,000 |               200,000 |						    650 |                   350 |                        40 |

*All prices/values are per house and will be increased with inflation each year*

You can adjust your policy to start any of these projects in any year in function `DetermineNewDevelopmentProjectsToExecute`.

## Step 3b: Invest in existing houses

The housing association may also invest in existing property they own. Houses have a lifespan in which they can be maintained for fairly low costs. If they exceed the lifespan, maintenance costs will increase considerably. By making certain investments housing associations can lengthen the lifespan of a house. These investments can also improve the sustainability of the houses, change maintenance costs (higher or lower), increase the market value and influence the rent. 

The housing association may also decide to renovate or even to rebuild the house. These investments are much more expensive but enables substantial improvements.

The following projects are available:

| **Project name**                | **Rent** |**Maximum allowed rent**| **Maintenance costs** | **Market value if empty** | **Lifespan** | **Sustainability** | **Construction costs ** |
| ------------------------------- | -------: | ---------------------: | --------------------: | ------------------------: | -----------: | ------------------------: | ---------: |
| **Solar panels**                |      +50 |                    +50 |                   +20 |                    +5,000 |            - |                       +15 |      8,000 |
| **Double glazed windows**       |        - |                    +25 |                   -50 |                         - |           +2 |                        +5 |      3,000 |
| **Major renovation**            |        - |                    +75 |                   -20 |                         - |          +20 |                       +10 |     15,000 |
| **Minor renovation**            |        - |                      - |                   -10 |                         - |           +4 |                        +4 |      3,500 |
| **Replacement of kitchen**      |        - |                      - |                     - |                    +1,000 |            - |                         - |      5,000 |
| **Rebuild flat**                |      400 |                    500 |                   300 |                    80,000 |           50 |                        50 |     65,000 |
| **Rebuild terraced house**      |      550 |                    650 |                   400 |                   120,000 |           50 |                        45 |    100,000 |
| **Rebuild semi-detached house** |      950 |                   1250 |                   600 |                   250,000 |           50 |                        35 |    225,000 |

*All prices/values are per house and will be increased with inflation each year*

You can choose them in function `DetermineRenovationsToExecute`.


