//
//  getIndices.swift
//  Sudoku
//
//  Created by Christopher Shipstone on 30/03/2026.
//

internal func getBlockIndices<T>(using generator: inout T) -> [Int] where T : RandomNumberGenerator {
    stride(from: 0, to: 9, by: 3)
        .shuffled(using: &generator)
}

internal func getCandidates<T>(using generator: inout T) -> [Int] where T : RandomNumberGenerator {
    (1...9).shuffled(using: &generator)
}

internal func getIndices<T>(using generator: inout T) -> [Int] where T : RandomNumberGenerator {
    (0..<81).shuffled(using: &generator)
}

internal func getHouseIndices<T>(using generator: inout T) -> [Int] where T : RandomNumberGenerator {
    (0..<9).shuffled(using: &generator)
}
