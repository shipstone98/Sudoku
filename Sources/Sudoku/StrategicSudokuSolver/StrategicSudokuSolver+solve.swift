//
//  StrategicSudokuSolver+solve.swift
//  Sudoku
//
//  Created by Christopher Shipstone on 30/03/2026.
//

public extension StrategicSudokuSolver {
    @discardableResult
    mutating func solve<T>(
        for strategy: SudokuSolverStrategy,
        using generator: inout T
    ) -> SudokuSolverMove? where T : RandomNumberGenerator {
        let move: SudokuSolverMove?
        
        switch strategy {
        case .fullHouse:
            move = self.solveFullHouse(using: &generator)
        case .nakedSingle:
            move = self.solveNakedSingle(using: &generator)
        case .hiddenSingle:
            move = self.solveHiddenSingle(using: &generator)
        case .pointingCandidate:
            move = self.solvePointingCandidate(using: &generator)
        case .claimingCandidate:
            move = self.solveClaimingCandidate(using: &generator)
        case .nakedPair:
            move = self.solveNakedPair(using: &generator)
        case .hiddenPair:
            move = self.solveHiddenPair(using: &generator)
        case .xWing:
            move = self.solveXWing(using: &generator)
        case .bugPlus1:
            move = self.solveBUGPlus1()
        case .xYWing:
            move = self.solveXYWing(using: &generator)
        }
        
        guard let move else {
            return nil
        }
        
        for location in move.locations {
            let index = location.row * 9 + location.column
            
            if let addedValue = location.addedValue {
                for index in getPeers(location.row, location.column) {
                    if var candidates = self.candidates[index] {
                        candidates.remove(addedValue)
                        self.candidates[index] = candidates
                    }
                }
                
                self.candidates.removeValue(forKey: index)
                self.sudoku.array[index] = addedValue
            } else {
                if var candidates = self.candidates[index] {
                    for candidate in location.removedCandidates {
                        candidates.remove(candidate)
                    }
                    
                    self.candidates[index] = candidates
                }
            }
        }
        
        self.moves.append(move)
        return move
    }
}
