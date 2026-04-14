//
//  StrategicSudokuSolver+solveXYWing.swift
//  Sudoku
//
//  Created by Christopher Shipstone on 31/03/2026.
//

import Utilities

internal extension StrategicSudokuSolver {
    private func getPincers(at index: Int, for candidate: Int) -> [(
        index: Int,
        candidate: Int
    )] {
        getPeers(index / 9, index % 9)
            .compactMap {
                currentIndex -> (index: Int, candidate: Int)? in
                guard currentIndex != index,
                      var candidates = self.candidates[currentIndex],
                      candidates.count == 2,
                      candidates.remove(candidate) != nil else {
                    return nil
                }
                
                return (currentIndex, candidates.first!)
            }
    }
    
    @inline(never)
    func solveXYWing<T>(using generator: inout T) -> SudokuSolverMove? where T : RandomNumberGenerator {
        for index in getIndices(using: &generator) {
            guard let candidates = self.candidates[index],
                  candidates.count == 2 else {
                continue
            }
            
            let candidate1 = candidates.first!
            let candidate2 = candidates.last!
            let candidate1Pincers = self.getPincers(at: index, for: candidate1)
            let candidate2Pincers = self.getPincers(at: index, for: candidate2)
            
            for (pincer1Index, pincer1Candidate) in candidate1Pincers {
                let removedCandidates: Set<Int> = [pincer1Candidate]
                
                for (pincer2Index, pincer2Candidate) in candidate2Pincers {
                    guard pincer1Candidate == pincer2Candidate else {
                        continue
                    }
                    
                    var locations: Set<SudokuSolverMove.Location> = []
                    
                    for currentIndex in 0..<81 {
                        guard currentIndex != index else {
                            continue
                        }
                        
                        let row = currentIndex / 9
                        let column = currentIndex % 9
                        let indices = getPeers(row, column)
                        
                        guard indices.contains(pincer1Index),
                              indices.contains(pincer2Index),
                              let candidates = self.candidates[currentIndex],
                              candidates.contains(pincer1Candidate) else {
                            continue
                        }
                        
                        let location = SudokuSolverMove.Location(
                            row,
                            column,
                            removedCandidates
                        )
                        
                        locations.insert(location)
                    }
                    
                    guard !locations.isEmpty else {
                        continue
                    }
                    
                    return .init(for: .xYWing, at: locations)
                }
            }
        }
        
        return nil
    }
}
